using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace CompanyApp.Entities
{
    [Table("Companys")]
    public class Company : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string TaxNumber { get; set; }

        [MaxLength(100)]
        public string TaxOffice { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string ContactPerson { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}