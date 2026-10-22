using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace CompanyApp.Companys.Dto
{
    [AutoMapTo(typeof(Entities.Company))]
    public class CreateCompanyDto
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

    }
}