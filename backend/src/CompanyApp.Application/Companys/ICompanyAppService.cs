using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.Companys.Dto;

namespace CompanyApp.Companys
{
    public interface ICompanyAppService : IAsyncCrudAppService<
        CompanyDto,
        long,
        PagedCompanyResultRequestDto,
        CreateCompanyDto,
        CompanyDto>
    {
        Task<CompanyReportDto> GetReportData(long id);
    }
}
