using System;
using Abp.Application.Services.Dto;

namespace CompanyApp.OrderLines.Dto
{
    public class PagedOrderLineResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? OrderId { get; set; }
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? LineOrder { get; set; }
        public decimal? QuantityFrom { get; set; }
        public decimal? QuantityTo { get; set; }
        public decimal? UnitPriceFrom { get; set; }
        public decimal? UnitPriceTo { get; set; }
        public decimal? DiscountRateFrom { get; set; }
        public decimal? DiscountRateTo { get; set; }
        public decimal? VatRateFrom { get; set; }
        public decimal? VatRateTo { get; set; }
        public decimal? TotalPriceFrom { get; set; }
        public decimal? TotalPriceTo { get; set; }
        public int? LineOrderFrom { get; set; }
        public int? LineOrderTo { get; set; }
    }
}
