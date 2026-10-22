using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace CompanyApp.Orders.Dto
{
    [AutoMapTo(typeof(Entities.Order))]
    public class CreateOrderDto
    {
        [Required]
        [MaxLength(50)]
        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal? DiscountRate { get; set; }

        public decimal? VatRate { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? VatAmount { get; set; }

        public decimal? GrandTotal { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }

    }
}