using System;
using Abp.Application.Services.Dto;

namespace CompanyApp.Orders.Dto
{
    public class PagedOrderResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? CompanyId { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? GrandTotal { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public DateTime? DeliveryDateFrom { get; set; }
        public DateTime? DeliveryDateTo { get; set; }
        public decimal? DiscountRateFrom { get; set; }
        public decimal? DiscountRateTo { get; set; }
        public decimal? VatRateFrom { get; set; }
        public decimal? VatRateTo { get; set; }
        public decimal? SubtotalFrom { get; set; }
        public decimal? SubtotalTo { get; set; }
        public decimal? DiscountAmountFrom { get; set; }
        public decimal? DiscountAmountTo { get; set; }
        public decimal? VatAmountFrom { get; set; }
        public decimal? VatAmountTo { get; set; }
        public decimal? GrandTotalFrom { get; set; }
        public decimal? GrandTotalTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
