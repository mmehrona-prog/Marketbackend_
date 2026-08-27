using MarketBackend.DTOs.Request;
using MarketBackend.Models;

namespace MarketBackend.Services.Auth
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateToken(User user);

        Task<bool> RegisterAsync(RegisterDto dto);
        Task<User> LoginAsync(LoginDto dto);
    }
}
