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
    public class CartController(ICartService cartService) : ControllerBase
    {
        //id берется из claims токена, который был установлен при аутентификации пользователя
        private int GetUserId()
        {
            var userIdClaim= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return 0;
            }
            return userId;
        }

        //добавление товаров в корзину
        [HttpPost("add")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0)
                {
                    var failResponse = APIResponse<string>.Fail("User unauthorized", HttpStatusCode.Unauthorized);
                    return Unauthorized(failResponse);
                }
                await cartService.AddToCartAsync(userId, request);

                var response = APIResponse<string>.Ok("Product added to cart successfully", HttpStatusCode.OK);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var respsponse= APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, respsponse);
            }
        }

        //удаление заказа из корзины
        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0)
                {
                    var  failResponse = APIResponse<object>.Fail("User unauthorized", HttpStatusCode.Unauthorized);
                    return Unauthorized(failResponse);
                }

                bool isRemoved = await cartService.RemoveFromCartAsync(userId, productId);
                if (!isRemoved)
                {
                   var failResponse = APIResponse<object>.Fail("Product not found in cart", HttpStatusCode.NotFound);
                    return NotFound(failResponse);
                }

                var response = APIResponse<string>.Ok("Product removed from cart successfully", HttpStatusCode.OK);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }

        //вывод содержимого корзины
        [HttpGet]
        public async Task<ActionResult> GetCart()
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0)
                {
                    var failResponse = APIResponse<object>.Fail("User unauthorized", HttpStatusCode.Unauthorized);
                    return Unauthorized(failResponse);
                }
                var cartData = await cartService.GetCartAsync(userId);

                var response= APIResponse<object>.Ok(cartData, HttpStatusCode.OK);
                return Ok(response);
            }
            catch(Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }

        //оформление заказа 
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout()
        {
            try
            {
                int userId = GetUserId();

                if (userId==0)
                {
                    var failResponse = APIResponse<object>.Fail("User unauthorized", HttpStatusCode.Unauthorized);
                    return Unauthorized(failResponse);
                }
                bool isSuccess= await cartService.CheckoutAsync(userId);
                if (isSuccess)
                {
                    var failResponse= APIResponse<object>.Fail("Cart is empty", HttpStatusCode.BadRequest);
                    return BadRequest(failResponse);
                }
                var response= APIResponse<string>.Ok("Checkout successful, your cart has been cleared", HttpStatusCode.OK);
                return Ok(response);
            }
                    
            catch (Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }
    }
}
