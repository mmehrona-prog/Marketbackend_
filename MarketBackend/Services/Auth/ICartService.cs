using MarketBackend.DTOs.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketBackend.DTOs.Response;

namespace MarketBackend.Services.Auth
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, AddToCartDto request);
        Task<bool> CheckoutAsync(int userId);
        Task<bool> RemoveFromCartAsync(int userId, int productId);

        Task<List<CartItemViewDto>> GetCartAsync(int userId);
    }
}
