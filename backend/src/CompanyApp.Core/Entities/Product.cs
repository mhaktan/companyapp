using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace CompanyApp.Entities
{
    [Table("Products")]
    public class Product : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; }

        public decimal? UnitPrice { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }
}