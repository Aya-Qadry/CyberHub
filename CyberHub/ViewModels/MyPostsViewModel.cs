using System.Collections.Generic;
using CyberHub.Models;

namespace CyberHub.ViewModels
{
    public class MyPostsViewModel
    {
        public User CurrentUser { get; set; }
        public List<Post> Posts { get; set; } = new();
    }
}
