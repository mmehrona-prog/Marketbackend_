using System;
using System.ComponentModel.DataAnnotations;
namespace MarketBackend.DTOs.Request
{
    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 150 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product description is required.")]
        [StringLength(1000, ErrorMessage = "Product description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0.")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        public string ImageUrl { get; set; }
    }
}
