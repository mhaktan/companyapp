using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace CompanyApp.Products.Dto
{
    [AutoMapFrom(typeof(Entities.Product))]
    public class ProductDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Unit { get; set; }

        public decimal? UnitPrice { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}