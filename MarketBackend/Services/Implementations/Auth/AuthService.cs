using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs.Request;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace MarketBackend.Services.Implementations.Auth
{
    public class AuthService(IConfiguration configuration, ApplicationDbContext context) : IAuthService
    {
        // Хэширование пароля 
        public string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
        // Проверка пароля
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
        // Генерация JWT токена
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Устанавливаем идентификацию пользователя и его роль в токене
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
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
        public async Task<bool>RegisterAsync(RegisterDto dto)
        {
            var userExists = await context.Users.AnyAsync(u=>u.Email == dto.Email);
            if (userExists) return false;
            var newuser = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = dto.Role.ToLower().Trim(),
            };
            

            context.Users.Add(newuser);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var user= await context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            if (user==null || !VerifyPassword(dto.Password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }
    }

}
