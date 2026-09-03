using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles ="admin")]
    public class AdminProductsController (IProductService productService) : ControllerBase
    {
        //добавление нового продукта
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] ProductCreateDto request)
        {
            try
            {
                await productService.CreateProductAsync(request);

                var response = APIResponse<object>.Ok("Product created successfully.", HttpStatusCode.Created);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }

        //редактирование продукта
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto request)
        {
            try
            {
                var updatedProduct= await productService.UpdateProductAsync(id, request);
                if(updatedProduct==null)
                {
                    var failReaponse = APIResponse<object>.Fail("Product not found.", HttpStatusCode.NotFound);
                    return NotFound(failReaponse);
                }

                var response = APIResponse<object>.Ok(updatedProduct, HttpStatusCode.OK);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }
        //удаление продукта
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                bool isFound = await productService.DeleteAsync(id);
                if (!isFound)
                {
                    var failResponse = APIResponse<object>.Fail("Product not found.", HttpStatusCode.NotFound);
                    return NotFound(failResponse);
                }

                var response = APIResponse<object>.Ok("Product deleted successfully.", HttpStatusCode.OK);
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }
    }
}
