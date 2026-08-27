using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController(ICartService cartService,ApplicationDbContext context, APIResponse APIResponse) : ControllerBase
    {
        //извлечение email пользователя из токена и находидт его Id в базе данных
        private int GetUserId()
        {
            var userEmail = User.Identity?.Name??
                User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return 0;

            var user = context.Users.FirstOrDefault(u => u.Email == userEmail);
            return user != null ? user.Id : 0;
        
        }

        //добавление товаров в корзину
        [HttpPost("add")]
        public async Task<ActionResult<APIResponse>> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.Unauthorized;
                    APIResponse.Data = null;
                    APIResponse.Error = "User not found or unauthorized.";
                    return Unauthorized(APIResponse);
                }
                    await cartService.AddToCartAsync(userId, request);

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product added to cart successfully.";
                APIResponse.Error = string.Empty;
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
                if (userId == 0)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.Unauthorized;
                    APIResponse.Data = null;
                    APIResponse.Error = "User not found or unauthorized.";
                    return Unauthorized(APIResponse);
                }

                bool isRemoved = await cartService.RemoveFromCartAsync(userId, productId);
                if (!isRemoved)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found in cart.";
                    return NotFound(APIResponse);
                    
                }

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product removed from cart successfully.";
                APIResponse.Error = string.Empty;

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

        //вывод содержимого корзины
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetCart()
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "User not found or unauthorized";
                    return Unauthorized(APIResponse);
                }
                var cartData = await cartService.GetCartAsync(userId);

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = cartData;
                APIResponse.Error = string.Empty;
                return Ok(APIResponse);
            }
            catch(Exception ex)
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

                bool isSuccess = await cartService.CheckoutAsync(userId);
                if (!isSuccess)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    APIResponse.Data = null;
                    APIResponse.Error = "Cart is empty.";
                    return BadRequest(APIResponse);
                }
                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Checkout successful. Your cart has been cleared";
                APIResponse.Error = string.Empty;

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
