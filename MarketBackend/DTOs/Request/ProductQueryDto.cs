using System;
using System.ComponentModel.DataAnnotations;

namespace MarketBackend.DTOs.Request
{
    public class ProductQueryDto
    {
        public string? Name { get; set; }


        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="The page number must be  no less than 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "The page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
    }
}
