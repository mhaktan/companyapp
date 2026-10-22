using System;
using System.Collections.Generic;
using CompanyApp.OrderLines.Dto;

namespace CompanyApp.Orders.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class OrderReportDto
    {
        public OrderDto Data { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
    }
}
