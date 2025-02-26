using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RazorWeb.Models
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
