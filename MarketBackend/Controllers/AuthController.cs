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
    public class AuthController(IAuthService authService) : ControllerBase
    {
        //регистрация
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                // Проверка, существует ли пользователь с таким email
                var authResult = await authService.RegisterAsync(request);

                if (authResult==null)
                {
                    var failResponse= APIResponse<string>.Fail("User with this email already exists.", HttpStatusCode.BadRequest);
                    return BadRequest(failResponse);
                }

                var response = APIResponse<AuthViewDto>.Ok(authResult, HttpStatusCode.OK);
                return Ok(response);
            }
            catch (Exception ex) {
                var response = APIResponse<object>.Fail(ex.Message, HttpStatusCode.InternalServerError);
                return StatusCode(500, response);
            }
        }
        //для входа в систему
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var authResult = await authService.LoginAsync(request);
                if (authResult  == null)
                {
                    var failResponse = APIResponse<object>.Fail("Invalid email or password.", HttpStatusCode.Unauthorized);
                    return Unauthorized(failResponse);
                }
               var response = APIResponse<AuthViewDto>.Ok(authResult, HttpStatusCode.OK);
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
