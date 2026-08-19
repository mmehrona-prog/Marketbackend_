using System;
using System.Net;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles ="admin")]
    public class AdminProductsController (ApplicationDbContext context, APIResponse APIResponse) : ControllerBase
    {
        //добавление нового продукта
        [HttpPost]
        public async Task<ActionResult<APIResponse>> AddProduct([FromBody] ProductCreateDto request)
        {
            try
            {
                var newProduct = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    ImageUrl = request.ImageUrl
                };
                context.Products.Add(newProduct);
                await context.SaveChangesAsync();
                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product added successfully.";
                APIResponse.Error = null;
                return Ok(APIResponse);
            }
            catch (Exception ex)
            {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Data = null;
                APIResponse.Error = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, APIResponse);
            }
        }
        //редактирование продукта
        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id, [FromBody] ProductUpdateDto request)
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
                //если продукт найден, обновляем его свойства
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
                return StatusCode((int)HttpStatusCode.InternalServerError, APIResponse);
            }
        }
        //удаление продукта
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
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
                return StatusCode((int)HttpStatusCode.InternalServerError, APIResponse);
            }
        }
    }
}
