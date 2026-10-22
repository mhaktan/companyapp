using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.Products.Dto;

namespace CompanyApp.Products
{
    public interface IProductAppService : IAsyncCrudAppService<
        ProductDto,
        long,
        PagedProductResultRequestDto,
        CreateProductDto,
        ProductDto>
    {
        decimal? GetStats(ProductStatsInput input);
        Task<ProductReportDto> GetReportData(long id);
    }
}
