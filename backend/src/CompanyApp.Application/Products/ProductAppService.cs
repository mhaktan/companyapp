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
using CompanyApp.Products.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.OrderLines.Dto;
using CompanyApp.Authorization;
using CompanyApp.Flows;

namespace CompanyApp.Products
{
    public class ProductAppService : AsyncCrudAppService<
        Product,
        ProductDto,
        long,
        PagedProductResultRequestDto,
        CreateProductDto,
        ProductDto>,
        IProductAppService
    {
        private readonly IFlowEngine _flowEngine;

        public ProductAppService(IRepository<Product, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Product_Read;
            GetAllPermissionName = PermissionNames.Product_Read;
            CreatePermissionName = PermissionNames.Product_Create;
            UpdatePermissionName = PermissionNames.Product_Update;
            DeletePermissionName = PermissionNames.Product_Delete;
        }

        protected override IQueryable<Product> CreateFilteredQuery(PagedProductResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Unit != null && x.Unit.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Unit.IsNullOrWhiteSpace(), x => x.Unit != null && x.Unit.Contains(input.Unit))
                .WhereIf(input.UnitPrice.HasValue, x => x.UnitPrice == input.UnitPrice.Value)
                .WhereIf(input.UnitPriceFrom.HasValue, x => x.UnitPrice >= input.UnitPriceFrom.Value)
                .WhereIf(input.UnitPriceTo.HasValue, x => x.UnitPrice <= input.UnitPriceTo.Value);
        }

        public override async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Product", result);
            return result;
        }

        public override async Task<ProductDto> UpdateAsync(ProductDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Product", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Product", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Product_Read)]
        public decimal? GetStats(ProductStatsInput input)
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

            var allowedNumeric = new[] { "UnitPrice" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "UnitPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.UnitPrice)
                            : query.Average(x => (decimal?)x.UnitPrice);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Product_Read)]
        public async Task<ProductReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.OrderLines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new ProductReportDto
            {
                Data = ObjectMapper.Map<ProductDto>(root),
                OrderLines = ObjectMapper.Map<List<OrderLineDto>>(
                    root.OrderLines == null ? new List<OrderLine>() : root.OrderLines.ToList()),
            };
        }

    }
}
