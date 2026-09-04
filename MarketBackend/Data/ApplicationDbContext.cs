using MarketBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Data
{
    public class ApplicationDbContext:DbContext
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options)
        {

        }
        //модели для базы данных
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        //уникальный индекс для Email
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //вызов базовой реализации
            base.OnModelCreating(modelBuilder);

            //задаем уникальность для колонки Email в таблице User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
        }
    }
}
