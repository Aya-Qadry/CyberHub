using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CyberHub.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Subject")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Snippet")]
        [StringLength(300)]
        public string? Snippet { get; set; }

        [Display(Name = "Post date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Update date")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Pinned")]
        public bool IsPinned { get; set; } = false;

        [Display(Name = "View count")]
        public int ViewCount { get; set; } = 0;

        [Required]
        public string AuthorId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
