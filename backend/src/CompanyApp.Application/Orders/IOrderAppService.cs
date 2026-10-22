using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.StateMachine.Dto;
using CompanyApp.Orders.Dto;

namespace CompanyApp.Orders
{
    public interface IOrderAppService : IAsyncCrudAppService<
        OrderDto,
        long,
        PagedOrderResultRequestDto,
        CreateOrderDto,
        OrderDto>
    {
        Task<OrderDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<OrderDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(OrderGroupedCountInput input);
        decimal? GetStats(OrderStatsInput input);
        Task<OrderReportDto> GetReportData(long id);
    }
}
