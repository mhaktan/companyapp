using AutoMapper;
using CompanyApp.Entities;
using CompanyApp.Orders.Dto;

namespace CompanyApp.Orders
{
    public class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateOrderDto, Order>();
            CreateMap<OrderDto, Order>();
        }
    }
}
