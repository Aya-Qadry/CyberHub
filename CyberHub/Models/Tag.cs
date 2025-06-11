using System.ComponentModel.DataAnnotations;

namespace CyberHub.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(100)]
        public string? Description { get; set; }


        //[Display(Name = "Couleur")]
        //[StringLength(7)]
        //public string? Color { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
