using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MarketBackend.Data;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace MarketBackend.Services.Implementations.Auth
{
    public class AuthService(IConfiguration configuration, ApplicationDbContext context, IMapper mapper) : IAuthService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();
        // Хэширование пароля 
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new User(),password);
        }
        // Проверка пароля
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(new User(), hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
        // Генерация JWT токена
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var jwtkey = configuration["Jwt:Key"]?? throw new InvalidOperationException("JWT key is not configured.");
            var key = Encoding.ASCII.GetBytes(jwtkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Устанавливаем идентификацию пользователя и его роль в токене
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                // Устанавливаем срок действия токена
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // Создаем токен
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
        //реализация регистрации 
        public async Task<AuthViewDto>RegisterAsync(RegisterDto dto)
        {
            var userExists = await context.Users.AnyAsync(u=>u.Email == dto.Email);
            if (userExists) return null;
            
            var newuser = mapper.Map<User>(dto);
            
            newuser.PasswordHash=HashPassword(dto.Password);
            newuser.Role = dto.Role.ToLower().Trim();

            context.Users.Add(newuser);
            await context.SaveChangesAsync();

            var response= mapper.Map<AuthViewDto>(dto);
            response.Token = GenerateToken(newuser);
            return response;
        }

        //реализация входа
        public async Task<AuthViewDto?> LoginAsync(LoginDto dto)
        {
            var user= await context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            if (user==null || !VerifyPassword(dto.Password, user.PasswordHash))
            {
                return null;
            }

            var response = mapper.Map<AuthViewDto>(dto);
            response.Token = GenerateToken(user);
            return response;
        }
    }
}