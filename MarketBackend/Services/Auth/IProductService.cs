using System.Collections.Generic;
using System.Threading.Tasks;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;

namespace MarketBackend.Services.Auth
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewDto>> GetAllProductsAsync(ProductQueryDto query);

        Task <ProductViewDto> CreateProductAsync (ProductCreateDto createDto);
        Task<ProductViewDto> UpdateProductAsync(int id, ProductUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
