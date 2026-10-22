using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace CompanyApp.Entities
{
    // State Machine: status — Draft → Confirmed → Cancelled
    // Initial: Draft | Transitions: Draft→Confirmed[Confirm], *→Cancelled[Cancel]
    [Table("Orders")]
    public class Order : FullAuditedEntity<long>
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

        public OrderStatus Status { get; set; }

        public long CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }
}