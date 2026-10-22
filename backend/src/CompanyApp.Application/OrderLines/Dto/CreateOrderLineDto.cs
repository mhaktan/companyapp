using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace CompanyApp.OrderLines.Dto
{
    [AutoMapTo(typeof(Entities.OrderLine))]
    public class CreateOrderLineDto
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? DiscountRate { get; set; }

        public decimal? VatRate { get; set; }

        public decimal? TotalPrice { get; set; }

        public int? LineOrder { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

    }
}