using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketBackend.Data;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;
using MarketBackend.Models;
using MarketBackend.Services.Auth;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Services.Implementations.Auth
{
    public class CartService(ApplicationDbContext context, IMapper mapper) : ICartService
    {
        //добавления товара в корзину
        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            var existingItem = await context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var newItem = mapper.Map<CartItem>(dto);
                newItem.UserId = userId;
                context.CartItems.Add(newItem);
            }
            await context.SaveChangesAsync();
        }

        //удаления товара из корзины
        public async Task<bool> RemoveFromCartAsync(int  userId, int productId)
        {
            var existingItem= await context.CartItems.FirstOrDefaultAsync(c=> c.UserId == userId && c.ProductId == productId);
            
            if(existingItem == null) return false;

            context.CartItems.Remove(existingItem);
            await context.SaveChangesAsync();
            return true;
        }
        //оформление заказа
        public async Task<bool> CheckoutAsync(int userId)
        {
            var cartItems= await context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            if (cartItems.Count == 0) return false;

            var productIds = cartItems.Select(c=> c.ProductId).ToList();
            var products = await context.Products.Where(p=>productIds.Contains(p.Id)).ToListAsync();

            decimal totalAmount = cartItems.Sum(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                return product != null ? product.Price * item.Quantity : 0;
            });
            var newOrder = new Order
            {
                UserId = userId,
                TotalPrice = totalAmount,
                CreatedAt = DateTime.UtcNow,
            };

            context.Orders.Add(newOrder);
            await context.SaveChangesAsync();

            var orderItems= cartItems.Select(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                return product != null ? new OrderItem
                {
                    OrderId = newOrder.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price,
                } : null;
            }).Where(oi => oi != null).ToList();

            context.OrderItems.AddRange(orderItems!);
            context.CartItems.RemoveRange(cartItems);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<List<CartItemViewDto>> GetCartAsync(int userId)
        {
            return await context.CartItems
                 .Where(c => c.UserId == userId)
                 .ProjectTo<CartItemViewDto>(mapper.ConfigurationProvider)
                 .ToListAsync();
        }
    }

}
