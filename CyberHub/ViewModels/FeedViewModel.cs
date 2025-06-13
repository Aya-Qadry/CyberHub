using CyberHub.Models;

namespace CyberHub.ViewModels
{
    public class FeedViewModel
    {
        public User CurrentUser { get; set; } = null!;
        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();

        public CreatePostViewModel NewPost { get; set; } = new CreatePostViewModel();  // 


    }
}
