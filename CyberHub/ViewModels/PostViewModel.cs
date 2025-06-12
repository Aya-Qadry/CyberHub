namespace CyberHub.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }


        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        //public string? UserProfilePicture { get; set; }

        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }


    }


}
