using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace CompanyApp.Orders.Dto
{
    [AutoMapFrom(typeof(Entities.Order))]
    public class OrderDto : EntityDto<long>
    {
        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal? DiscountRate { get; set; }

        public decimal? VatRate { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? VatAmount { get; set; }

        public decimal? GrandTotal { get; set; }

        public string Notes { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long CompanyId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}