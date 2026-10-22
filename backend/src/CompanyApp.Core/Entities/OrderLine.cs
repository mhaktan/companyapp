using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace CompanyApp.Entities
{
    [Table("OrderLines")]
    public class OrderLine : FullAuditedEntity<long>
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

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

    }
}