using AutoMapper;
using CompanyApp.Entities;
using CompanyApp.Products.Dto;

namespace CompanyApp.Products
{
    public class ProductMapProfile : Profile
    {
        public ProductMapProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<ProductDto, Product>();
        }
    }
}
