using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace MarketBackend.CartControllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        //просмотр всех продуктов с фильтрации по имени и цене
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll([FromQuery] ProductQueryDto query)
        {
            try
            {
                var result = await productService.GetAllProductsAsync(query);

               var response= APIResponse<object>.Ok(result, HttpStatusCode.OK);

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
