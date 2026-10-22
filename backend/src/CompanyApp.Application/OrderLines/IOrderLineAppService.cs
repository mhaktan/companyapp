using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.OrderLines.Dto;

namespace CompanyApp.OrderLines
{
    public interface IOrderLineAppService : IAsyncCrudAppService<
        OrderLineDto,
        long,
        PagedOrderLineResultRequestDto,
        CreateOrderLineDto,
        OrderLineDto>
    {
        List<GroupCountDto> GetGroupedCount(OrderLineGroupedCountInput input);
        decimal? GetStats(OrderLineStatsInput input);
    }
}
