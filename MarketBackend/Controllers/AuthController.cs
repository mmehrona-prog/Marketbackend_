using System;
using System.Net;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MarketBackend.Services.Auth;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService,
            APIResponse APIResponse) : ControllerBase
    {
        //регистрация
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterDto request)
        {
            try
            {
                // Проверка, существует ли пользователь с таким email
                bool isRegistered = await authService.RegisterAsync(request);

                if (!isRegistered)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    APIResponse.Data = null;
                    APIResponse.Error = "User with this email already exists.";
                    return BadRequest(APIResponse);
                }

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "User registered successfully.";
                APIResponse.Error = string.Empty;

                return Ok(APIResponse);
            }
            catch (Exception ex) {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Data = null;
                APIResponse.Error = ex.Message;
                return StatusCode(500, APIResponse);
            }
        }
        //для входа в систему
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginDto request)
        {
            try
            {
                var user = await authService.LoginAsync(request);
                if (user == null)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.Unauthorized;
                    APIResponse.Data = null;
                    APIResponse.Error = "Invalid email or password.";
                    return Unauthorized(APIResponse);
                }
                // Генерация JWT токена
                var token = authService.GenerateToken(user);

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;

                APIResponse.Data = new AuthViewDto
                {
                    Token = token,
                    Email = user.Email,
                    Role = user.Role,
                };

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
