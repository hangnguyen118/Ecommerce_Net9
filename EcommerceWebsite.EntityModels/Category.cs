using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebsite.EntityModels
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(300)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [MaxLength(300)]
        [DisplayName("Description")]
        public string Description { get; set; }
    }
}
