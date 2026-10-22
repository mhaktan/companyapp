using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace CompanyApp.Products.Dto
{
    [AutoMapTo(typeof(Entities.Product))]
    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; }

        public decimal? UnitPrice { get; set; }

    }
}