using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController(ApplicationDbContext context, APIResponse APIResponse) : ControllerBase
    {
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim !=null? int.Parse(userIdClaim.Value):0;
        }

        [HttpPost("add")]
        public async Task<ActionResult<APIResponse>> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                int userId = GetUserId();
                var existingItem = await context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;

                }
                
                else
                {
                    var newItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    };
                    context.CartItems.Add(newItem);
                }
                await context.SaveChangesAsync();

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product added to cart successfully.";
                APIResponse.Error = null;
                return Ok(APIResponse);
            }
            catch (Exception ex)
            {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Data = null;
                APIResponse.Error = ex.Message;
                return StatusCode(500, APIResponse);
            }
        }
        //удаление заказа из корзины
        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<APIResponse>> RemoveFromCart(int productId)
        {
            try
            {
                int userId = GetUserId();
              
                var existingItem = await context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
                if (existingItem != null)
                {
                    context.CartItems.Remove(existingItem);
                    await context.SaveChangesAsync();
                    APIResponse.Status = true;
                    APIResponse.StatusCode = HttpStatusCode.OK;
                    APIResponse.Data = "Product removed from cart successfully.";
                    APIResponse.Error = null;
                    return Ok(APIResponse);
                }
                else
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found in cart.";
                    return NotFound(APIResponse);
                }
            }
            catch (Exception ex)
            {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Data = null;
                APIResponse.Error = ex.Message;
                return StatusCode(500, APIResponse);
            }
        }
        //оформление заказа 
        [HttpPost("checkout")]
        public async Task<ActionResult<APIResponse>> Checkout()
        {
            try
            {
                int userId = GetUserId();
                //вывод товаров из корзины пользователя
                var cartItems = await context.CartItems.Where(c => c.UserId == userId).ToListAsync();
                if (cartItems.Count == 0)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    APIResponse.Data = null;
                    APIResponse.Error = "Cart is empty.";
                    return BadRequest(APIResponse);
                }
                // актулаьные цены продуктов
                var productIds = cartItems.Select(c => c.ProductId).ToList();
                var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                //общяя стоимость заказа
                decimal totalAmount = 0;
                foreach (var item in cartItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        totalAmount += product.Price * item.Quantity;
                    }
                }
                //создание записи заказа
                var newOrder = new Order
                {
                    UserId = userId,
                    TotalPrice = totalAmount,
                    CreatedAt = DateTime.UtcNow
                };
                context.Orders.Add(newOrder);
                await context.SaveChangesAsync();

                //передача данных о заказе в платежную систему
                foreach (var item in cartItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = newOrder.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            Price = product.Price
                        };
                        context.OrderItems.Add(orderItem);
                    }
                    //удаление товаров из корзины после оформления заказа
                    context.CartItems.Remove(item);
                }
                    //сохранение изменений в базе данных
                    await context.SaveChangesAsync();

                    APIResponse.Status = true;
                    APIResponse.StatusCode = HttpStatusCode.OK;
                    APIResponse.Data = "Checkout successful.";
                    APIResponse.Error = null;

                    return Ok(APIResponse);
            }
            catch (Exception ex)
            {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Data = null;
                APIResponse.Error = ex.Message;
                return StatusCode(500, APIResponse);
            }
        }
    }
}
