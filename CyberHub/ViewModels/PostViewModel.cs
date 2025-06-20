namespace CyberHub.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        //public string? UserProfilePicture { get; set; }

        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public string? UserProfileImageUrl { get; set; } // Add this line

        public bool IsLikedByCurrentUser { get; set; }

        // nv
        public string CategoryName { get; set; }
        public List<string> Tags { get; set; }
        public string ImageUrl { get; set; }

        public List<CommentViewModel> Comments { get; set; } = new();


    }


}
