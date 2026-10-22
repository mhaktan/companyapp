using System;
using System.Collections.Generic;
using CompanyApp.Orders.Dto;

namespace CompanyApp.Companys.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class CompanyReportDto
    {
        public CompanyDto Data { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}
