using AutoMapper;
using MarketBackend.Models;
using MarketBackend.DTOs.Request;
using MarketBackend.DTOs.Response;


namespace MarketBackend.Mapping
{
    public class MappingProfile:Profile
    {
       public MappingProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap< Product, ProductViewDto>();

            CreateMap<AddToCartDto, CartItem>();
            CreateMap<CartItem, CartItemViewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown"))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty))
                .ForMember(dest => dest.TotalItemPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price * src.Quantity : 0));

            CreateMap<RegisterDto, User>();
            CreateMap<User, AuthViewDto>();
        }
    }
}
