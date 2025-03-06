using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.EntityModels
{
    public class Product
    {
        [Key]
        public int Id {  get; set; }

        [Required]
        [MaxLength(300)]
        [DisplayName("Product Title")]
        public string Title { get; set; }

        [MaxLength(500)]
        [DisplayName("Product Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Product List Price")]
        [Range(1000, 2000000000)]
        public double Price { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string? ImageURL {  get; set; }
    }
}
