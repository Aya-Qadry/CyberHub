using System.ComponentModel.DataAnnotations;

namespace CyberHub.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Post content is required")]
        [StringLength(2000, ErrorMessage = "Post content cannot exceed 2000 characters")]
        [MinLength(1, ErrorMessage = "Post content cannot be empty")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string Tags { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }
    }
}
