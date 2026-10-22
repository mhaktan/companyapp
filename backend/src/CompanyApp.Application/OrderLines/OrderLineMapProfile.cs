using AutoMapper;
using CompanyApp.Entities;
using CompanyApp.OrderLines.Dto;

namespace CompanyApp.OrderLines
{
    public class OrderLineMapProfile : Profile
    {
        public OrderLineMapProfile()
        {
            CreateMap<OrderLine, OrderLineDto>();
            CreateMap<CreateOrderLineDto, OrderLine>();
            CreateMap<OrderLineDto, OrderLine>();
        }
    }
}
