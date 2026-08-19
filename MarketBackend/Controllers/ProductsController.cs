using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace MarketBackend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController(ApplicationDbContext context, APIResponse APIResponse) : ControllerBase
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
                var products = await context.Products.AsQueryable()
                    .WhereIf(!string.IsNullOrEmpty(name), p => p.Name.ToLower().Contains(name!.ToLower()))
                    .WhereIf(minPrice.HasValue, p => p.Price >= minPrice.Value)
                    .WhereIf(maxPrice.HasValue, p => p.Price <= maxPrice.Value)
                    .ToListAsync();
                // Преобразуем список продуктов в список DTO для ответа
                var result = products.Select(p => new ProductViewDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = (decimal)(p.Price),
                    ImageUrl = p.ImageUrl
                }).ToList();

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = result;
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
        //создание нового продукта (только для админа)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> Create([FromBody] ProductCreateDto request)
        {
            try
            {
                var newproduct = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    ImageUrl = request.ImageUrl,
                    CreatedAt = DateTime.Now,
                };
                context.Products.Add(newproduct);
                await context.SaveChangesAsync();


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
                var product = await context.Products.FindAsync(id);
                if (product == null)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found.";
                    return NotFound(APIResponse);
                }
                // Обновляем свойства продукта
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.ImageUrl = request.ImageUrl;

                await context.SaveChangesAsync();

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product updated successfully.";
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
        //удаление продукта (только для админа)
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> Delete(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                if (product == null)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found.";
                    return NotFound(APIResponse);
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product deleted successfully.";
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
