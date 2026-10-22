using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace CompanyApp.OrderLines.Dto
{
    [AutoMapFrom(typeof(Entities.OrderLine))]
    public class OrderLineDto : EntityDto<long>
    {
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? DiscountRate { get; set; }

        public decimal? VatRate { get; set; }

        public decimal? TotalPrice { get; set; }

        public int? LineOrder { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}