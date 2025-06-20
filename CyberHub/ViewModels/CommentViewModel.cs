namespace CyberHub.ViewModels
{
    public class CommentViewModel
    {
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AuthorProfileImageUrl { get; set; } // Add this line

    }
}
