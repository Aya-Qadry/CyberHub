using System.ComponentModel.DataAnnotations;

namespace CyberHub.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Post content is required")]
        [StringLength(2000, ErrorMessage = "Post content cannot exceed 2000 characters")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string? Tags { get; set; }
    }
}
