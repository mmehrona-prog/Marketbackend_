using System;
using System.Net;
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
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService,
            ApplicationDbContext context,
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
                var user = await context.Users.AnyAsync(u => u.Email == request.Email);
                if (user)
                {
                    APIResponse.Status = false;
                    APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    APIResponse.Data = null;
                    APIResponse.Error = "User with this email already exists.";
                    return BadRequest(APIResponse);
                }

                var newUser = new User
                {
                    Email = request.Email,
                    PasswordHash = authService.HashPassword(request.Password),
                    Role = "user"
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                APIResponse.Status = true;
                APIResponse.StatusCode = HttpStatusCode.OK;
                APIResponse.Data = "User registered successfully.";
                APIResponse.Error = null;

                return Ok(APIResponse);
            }
            catch (Exception ex) {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Error = ex.Data;
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
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || !authService.VerifyPassword(request.Password, user.PasswordHash))
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
                APIResponse.Data = new { Token = token };
                APIResponse.Error = null;
                return Ok(APIResponse);
            }
            catch (Exception ex)
            {
                APIResponse.Status = false;
                APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                APIResponse.Error = ex.Data;
                return StatusCode(500, APIResponse);
            }
        }
    }

}
