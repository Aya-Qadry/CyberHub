using System.Diagnostics;
using CyberHub.Data;
using CyberHub.Models;
using CyberHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Main()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20) // 20 most recent posts
                .ToListAsync();

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.AuthorId,
                UserDisplayName = p.Author.UserName ?? "Unknown User",
                LikesCount = 0,  
                CommentsCount = p.Comments.Count
            }).ToList();

            var feedViewModel = new FeedViewModel
            {
                CurrentUser = currentUser,
                Posts = postViewModels
            };

            return View(feedViewModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Main");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var post = new Post
            {
                Subject = model.Content.Length > 50 ? model.Content.Substring(0, 50) + "..." : model.Content,
                Content = model.Content,
                Snippet = model.Content.Length > 300 ? model.Content.Substring(0, 300) + "..." : model.Content,
                AuthorId = currentUser.Id,
                CategoryId = 1, 
                CreatedAt = DateTime.UtcNow,
                IsPublished = true
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(model.Tags))
            {
                var tagNames = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

                foreach (var tagName in tagNames)
                {
                    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (existingTag == null)
                    {
                        existingTag = new Tag { Name = tagName };
                        _context.Tags.Add(existingTag);
                        await _context.SaveChangesAsync();
                    }

                    var postTag = new PostTag
                    {
                        PostId = post.Id,
                        TagId = existingTag.Id
                    };
                    _context.PostTags.Add(postTag);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Main");
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}