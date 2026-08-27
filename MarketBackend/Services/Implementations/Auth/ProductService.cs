using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Data;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Services.Implementations.Auth
{
    public class ProductService(ApplicationDbContext context) : IProductService
    {
        //просмотр 
        public async Task<List<ProductViewDto>> GetAllAsync(string? name, decimal? minPrice, decimal? maxPrice)
        {
            var products = await context.Products.AsQueryable()
                .WhereIf(!string.IsNullOrEmpty(name), p => p.Name.ToLower().Contains(name!.ToLower()))
                .WhereIf(minPrice.HasValue, p => p.Price >= minPrice.Value)
                .WhereIf(maxPrice.HasValue, p => p.Price <= maxPrice.Value)
                .ToListAsync();
            return products.Select(p => new ProductViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = (decimal)(p.Price),
                ImageUrl = p.ImageUrl
            }).ToList();
        }
        //создание товара админом
        public async Task CreateAsync(ProductCreateDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };
            context.Products.Add(newProduct);
            await context.SaveChangesAsync();
        }
        //редактирование товара админом
        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await context.Products.FindAsync(id);
            //если товар не найден в бд и выводим false
            if (product == null)
            {
                return false;
            }
            //если найден перезаписваем его свойства
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.ImageUrl = dto.ImageUrl;
            
            await context.SaveChangesAsync();
            return true;
        }
        //удаление товара админом
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
    }

}
