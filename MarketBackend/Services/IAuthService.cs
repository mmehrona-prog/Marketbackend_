using MarketBackend.Models;

namespace MarketBackend.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateToken(User user);
    }
}
