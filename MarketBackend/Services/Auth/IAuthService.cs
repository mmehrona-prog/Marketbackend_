using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using System.Threading.Tasks;

namespace MarketBackend.Services.Auth
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateToken(User user);

        Task<AuthViewDto> RegisterAsync(RegisterDto dto);
        Task<AuthViewDto> LoginAsync(LoginDto dto);
    }
}
