using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace CyberHub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(200)]
        public string? Description { get; set; }

        //[Display(Name = "Couleur")]
        //[StringLength(7)] 
        //public string? Color { get; set; }

        [Display(Name = "Icon")]
        [StringLength(50)]
        public string? Icon { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
