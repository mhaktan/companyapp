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
using CompanyApp.Companys.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.Orders.Dto;
using CompanyApp.Authorization;
using CompanyApp.Flows;

namespace CompanyApp.Companys
{
    public class CompanyAppService : AsyncCrudAppService<
        Company,
        CompanyDto,
        long,
        PagedCompanyResultRequestDto,
        CreateCompanyDto,
        CompanyDto>,
        ICompanyAppService
    {
        private readonly IFlowEngine _flowEngine;

        public CompanyAppService(IRepository<Company, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Company_Read;
            GetAllPermissionName = PermissionNames.Company_Read;
            CreatePermissionName = PermissionNames.Company_Create;
            UpdatePermissionName = PermissionNames.Company_Update;
            DeletePermissionName = PermissionNames.Company_Delete;
        }

        protected override IQueryable<Company> CreateFilteredQuery(PagedCompanyResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.TaxNumber != null && x.TaxNumber.Contains(input.Keyword)) ||
                    (x.TaxOffice != null && x.TaxOffice.Contains(input.Keyword)) ||
                    (x.Address != null && x.Address.Contains(input.Keyword)) ||
                    (x.Phone != null && x.Phone.Contains(input.Keyword)) ||
                    (x.Email != null && x.Email.Contains(input.Keyword)) ||
                    (x.ContactPerson != null && x.ContactPerson.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.TaxNumber.IsNullOrWhiteSpace(), x => x.TaxNumber != null && x.TaxNumber.Contains(input.TaxNumber))
                .WhereIf(!input.TaxOffice.IsNullOrWhiteSpace(), x => x.TaxOffice != null && x.TaxOffice.Contains(input.TaxOffice))
                .WhereIf(!input.Address.IsNullOrWhiteSpace(), x => x.Address != null && x.Address.Contains(input.Address))
                .WhereIf(!input.Phone.IsNullOrWhiteSpace(), x => x.Phone != null && x.Phone.Contains(input.Phone))
                .WhereIf(!input.Email.IsNullOrWhiteSpace(), x => x.Email != null && x.Email.Contains(input.Email))
                .WhereIf(!input.ContactPerson.IsNullOrWhiteSpace(), x => x.ContactPerson != null && x.ContactPerson.Contains(input.ContactPerson));
        }

        public override async Task<CompanyDto> CreateAsync(CreateCompanyDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Company", result);
            return result;
        }

        public override async Task<CompanyDto> UpdateAsync(CompanyDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Company", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Company", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Company_Read)]
        public async Task<CompanyReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new CompanyReportDto
            {
                Data = ObjectMapper.Map<CompanyDto>(root),
                Orders = ObjectMapper.Map<List<OrderDto>>(
                    root.Orders == null ? new List<Order>() : root.Orders.ToList()),
            };
        }

    }
}
