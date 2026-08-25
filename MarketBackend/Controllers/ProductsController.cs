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
        //создание нового продукта (только для админа)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> Create([FromBody] ProductCreateDto request)
        {
            try
            {
                await productService.CreateAsync(request);

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.Created;
                APIResponse.Data = "Product created successfully.";
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
        //обновление продукта (только для админа)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] ProductCreateDto request)
        {
            try
            {
                bool isFound = await productService.UpdateAsync(id, request);
                if (isFound)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found";
                    return NotFound( APIResponse);
                } 

                    APIResponse.Status = true;
                    APIResponse.StatusCode = HttpStatusCode.OK;
                    APIResponse.Data = "Product updated successfully.";
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
        //удаление продукта (только для админа)
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> Delete(int id)
        {
            try
            {
               bool isFound = await productService.DeleteAsync(id);
                if(isFound)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found.";
                    return NotFound(APIResponse);
                }

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product deleted successfully.";
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
