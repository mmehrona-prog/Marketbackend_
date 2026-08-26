using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.AspNetCore.Authentication;
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
    public class AdminProductsController (IProductService productService, APIResponse APIResponse) : ControllerBase
    {
        //добавление нового продукта
        [HttpPost]
        public async Task<ActionResult<APIResponse>> AddProduct([FromBody] ProductCreateDto request)
        {
            try
            {
                await productService.CreateAsync(request);


                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "Product added successfully.";
                APIResponse.Error = string.Empty;
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
                bool isFound = await productService.UpdateAsync(id, request);
                if (!isFound)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.NotFound;
                    APIResponse.Data = null;
                    APIResponse.Error = "Product not found.";
                    return NotFound(APIResponse);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, APIResponse);
            }
        }
        //удаление продукта
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
        {
            try
            {
                bool isFound = await productService.DeleteAsync(id);
                if (!isFound)
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
                return StatusCode((int)HttpStatusCode.InternalServerError, APIResponse);
            }
        }
    }
}
