using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using CompanyApp.Entities;
using CompanyApp.OrderLines.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.Authorization;
using CompanyApp.Flows;

namespace CompanyApp.OrderLines
{
    public class OrderLineAppService : AsyncCrudAppService<
        OrderLine,
        OrderLineDto,
        long,
        PagedOrderLineResultRequestDto,
        CreateOrderLineDto,
        OrderLineDto>,
        IOrderLineAppService
    {
        private readonly IFlowEngine _flowEngine;

        public OrderLineAppService(IRepository<OrderLine, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.OrderLine_Read;
            GetAllPermissionName = PermissionNames.OrderLine_Read;
            CreatePermissionName = PermissionNames.OrderLine_Create;
            UpdatePermissionName = PermissionNames.OrderLine_Update;
            DeletePermissionName = PermissionNames.OrderLine_Delete;
        }

        protected override IQueryable<OrderLine> CreateFilteredQuery(PagedOrderLineResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.Unit != null && x.Unit.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.Unit.IsNullOrWhiteSpace(), x => x.Unit != null && x.Unit.Contains(input.Unit))
                .WhereIf(input.Quantity.HasValue, x => x.Quantity == input.Quantity.Value)
                .WhereIf(input.UnitPrice.HasValue, x => x.UnitPrice == input.UnitPrice.Value)
                .WhereIf(input.DiscountRate.HasValue, x => x.DiscountRate == input.DiscountRate.Value)
                .WhereIf(input.VatRate.HasValue, x => x.VatRate == input.VatRate.Value)
                .WhereIf(input.TotalPrice.HasValue, x => x.TotalPrice == input.TotalPrice.Value)
                .WhereIf(input.LineOrder.HasValue, x => x.LineOrder == input.LineOrder.Value)
                .WhereIf(input.QuantityFrom.HasValue, x => x.Quantity >= input.QuantityFrom.Value)
                .WhereIf(input.QuantityTo.HasValue, x => x.Quantity <= input.QuantityTo.Value)
                .WhereIf(input.UnitPriceFrom.HasValue, x => x.UnitPrice >= input.UnitPriceFrom.Value)
                .WhereIf(input.UnitPriceTo.HasValue, x => x.UnitPrice <= input.UnitPriceTo.Value)
                .WhereIf(input.DiscountRateFrom.HasValue, x => x.DiscountRate >= input.DiscountRateFrom.Value)
                .WhereIf(input.DiscountRateTo.HasValue, x => x.DiscountRate <= input.DiscountRateTo.Value)
                .WhereIf(input.VatRateFrom.HasValue, x => x.VatRate >= input.VatRateFrom.Value)
                .WhereIf(input.VatRateTo.HasValue, x => x.VatRate <= input.VatRateTo.Value)
                .WhereIf(input.TotalPriceFrom.HasValue, x => x.TotalPrice >= input.TotalPriceFrom.Value)
                .WhereIf(input.TotalPriceTo.HasValue, x => x.TotalPrice <= input.TotalPriceTo.Value)
                .WhereIf(input.LineOrderFrom.HasValue, x => x.LineOrder >= input.LineOrderFrom.Value)
                .WhereIf(input.LineOrderTo.HasValue, x => x.LineOrder <= input.LineOrderTo.Value)
                .WhereIf(input.OrderId.HasValue, x => x.OrderId == input.OrderId.Value)
                .WhereIf(input.ProductId.HasValue, x => x.ProductId == input.ProductId.Value);
        }

        public override async Task<OrderLineDto> CreateAsync(CreateOrderLineDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "OrderLine", result);
            return result;
        }

        public override async Task<OrderLineDto> UpdateAsync(OrderLineDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "OrderLine", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "OrderLine", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.OrderLine_Read)]
        public List<GroupCountDto> GetGroupedCount(OrderLineGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "OrderId", "ProductId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "OrderId":
                    return query
                        .GroupBy(x => new { Key = x.OrderId, Label = x.Order == null ? null : x.Order.OrderNo })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "ProductId":
                    return query
                        .GroupBy(x => new { Key = x.ProductId, Label = x.Product == null ? null : x.Product.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.OrderLine_Read)]
        public decimal? GetStats(OrderLineStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Quantity", "UnitPrice", "DiscountRate", "VatRate", "TotalPrice", "LineOrder" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Quantity": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Quantity)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Quantity)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Quantity)
                            : query.Average(x => (decimal?)x.Quantity);
                        case "UnitPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.UnitPrice)
                            : query.Average(x => (decimal?)x.UnitPrice);
                        case "DiscountRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.DiscountRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.DiscountRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.DiscountRate)
                            : query.Average(x => (decimal?)x.DiscountRate);
                        case "VatRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.VatRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.VatRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.VatRate)
                            : query.Average(x => (decimal?)x.VatRate);
                        case "TotalPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalPrice)
                            : query.Average(x => (decimal?)x.TotalPrice);
                        case "LineOrder": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.LineOrder)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.LineOrder)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.LineOrder)
                            : query.Average(x => (decimal?)x.LineOrder);
                        default: return null;
            }
        }

    }
}
