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


namespace MarketBackend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController(IProductService productService, APIResponse APIResponse) : ControllerBase
    {
        //просмотр всех продуктов с фильтрации по имени и цене
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> GetAll(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            try
            {
                var result = await productService.GetAllAsync(name, minPrice, maxPrice);

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = result;
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
