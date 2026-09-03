using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketBackend.Data;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Services.Implementations.Auth
{
    public class ProductService(ApplicationDbContext context, IMapper mapper) : IProductService
    {
        //просмотр 
        public async Task<IEnumerable<ProductViewDto>> GetAllProductsAsync(ProductQueryDto query)
        {
            var dbQuery = context.Products.AsQueryable();
            //фильтрация по имени и цене
            if (!string.IsNullOrEmpty(query.Name))
            {
                dbQuery = dbQuery.Where(p => p.Name.Contains(query.Name));
            }

            if(query.MinPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Price <= query.MaxPrice.Value);
            }

            //пагинация
            var skipAmount = (query.PageNumber - 1) * query.PageSize;

            var products = await dbQuery
                .Skip(skipAmount)
                .Take(query.PageSize)
                .ToListAsync();
            return mapper.Map<IEnumerable<ProductViewDto>>(products);
        }
        //создание товара админом
        public async Task<ProductViewDto> CreateProductAsync(ProductCreateDto dto)
        {
            var newProduct = mapper.Map<Product>(dto);
            newProduct.CreatedAt=DateTime.UtcNow;

            context.Products.Add(newProduct);
            await context.SaveChangesAsync();

            return mapper.Map<ProductViewDto>(newProduct);
        }
        //редактирование товара админом
        public async Task<ProductViewDto> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await context.Products.FindAsync(id);
            //если товар не найден в бд и выводим false
            if (product == null)
            {
                return null;
            }
            //если найден перезаписваем его свойства
                mapper.Map(dto,product);
            await context.SaveChangesAsync();
            return mapper.Map<ProductViewDto>(product);
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
