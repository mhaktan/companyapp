using System;
using CompanyApp.Analytics.Dto;

namespace CompanyApp.Orders.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class OrderGroupedCountInput : PagedOrderResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class OrderStatsInput : PagedOrderResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
