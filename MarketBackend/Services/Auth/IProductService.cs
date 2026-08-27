using System.Collections.Generic;
using System.Threading.Tasks;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;

namespace MarketBackend.Services.Auth
{
    public interface IProductService
    {
        Task<List<ProductViewDto>> GetAllAsync(string? name, decimal? minPrice, decimal? maxPrice);

        Task CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(int id, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
