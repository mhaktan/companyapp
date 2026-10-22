using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace CompanyApp.Companys.Dto
{
    [AutoMapFrom(typeof(Entities.Company))]
    public class CompanyDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string TaxNumber { get; set; }

        public string TaxOffice { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string ContactPerson { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}