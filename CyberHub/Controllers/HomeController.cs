using CyberHub.Data;
using CyberHub.Models;
using CyberHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

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

        //public IActionResult Index()
        //{
        //    return View();
        //}


        public async Task<IActionResult> Main()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = currentUser.Id;

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                 .Include(p => p.PostLikes)
                 .Include(p => p.Comments)
                       .ThenInclude(c => c.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.AuthorId,
                UserDisplayName = p.Author.UserName ?? "Unknown User",
                UserProfileImageUrl = p.Author.ProfilePictureUrl, 
                LikesCount = p.PostLikes.Count,
                CommentsCount = p.Comments.Count,
                CategoryName = p.Category?.Name ?? "Other",
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                ImageUrl = p.ImageUrl,
                IsLikedByCurrentUser = p.PostLikes.Any(pl => pl.UserId == userId),

                Comments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        Content = c.Content,
                        AuthorName = c.Author.UserName ?? "User",
                        AuthorProfileImageUrl = c.Author.ProfilePictureUrl,  
                        CreatedAt = c.CreatedAt
                    }).ToList()
            }).ToList();

            var trendingTags = await _context.Tags
                .Select(tag => new
                {
                    tag.Name,
                    PostCount = tag.PostTags.Count()
                })
                .OrderByDescending(t => t.PostCount)
                .Take(5)
                .ToListAsync();

                    var trendingTagVMs = trendingTags
                        .Select((t, i) => new TrendingTagViewModel
                        {
                            Rank = i + 1,
                            Name = t.Name,
                            PostCount = t.PostCount
                        }).ToList();

            var categories = await _context.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();


            var feedViewModel = new FeedViewModel
            {
                CurrentUser = currentUser,
                Posts = postViewModels,
                NewPost = new CreatePostViewModel(),
                TrendingTags = trendingTagVMs,
                Categories = categories 
            };

            return View(feedViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Tag(string name)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return RedirectToAction("Main");
            }

            var userId = currentUser.Id;

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.PostLikes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .Where(p => p.IsPublished && p.PostTags.Any(pt => pt.Tag.Name == name))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.AuthorId,
                UserDisplayName = p.Author.UserName ?? "Unknown User",
                UserProfileImageUrl = p.Author.ProfilePictureUrl,
                LikesCount = p.PostLikes.Count,
                CommentsCount = p.Comments.Count,
                CategoryName = p.Category?.Name ?? "Other",
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                ImageUrl = p.ImageUrl,
                IsLikedByCurrentUser = p.PostLikes.Any(pl => pl.UserId == userId),

                Comments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        Content = c.Content,
                        AuthorName = c.Author.UserName ?? "User",
                        AuthorProfileImageUrl = c.Author.ProfilePictureUrl,
                        CreatedAt = c.CreatedAt
                    }).ToList()
            }).ToList();

            var trendingTags = await _context.Tags
                .Select(tag => new
                {
                    tag.Name,
                    PostCount = tag.PostTags.Count()
                })
                .OrderByDescending(t => t.PostCount)
                .Take(5)
                .ToListAsync();

            var trendingTagVMs = trendingTags
                .Select((t, i) => new TrendingTagViewModel
                {
                    Rank = i + 1,
                    Name = t.Name,
                    PostCount = t.PostCount
                }).ToList();
            var categories = await _context.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            var feedViewModel = new FeedViewModel
            {
                CurrentUser = currentUser,
                Posts = postViewModels,
                NewPost = new CreatePostViewModel(),
                TrendingTags = trendingTagVMs,
                Categories = categories  

            };

            return View("Main", feedViewModel); 
        }


        [HttpGet]
        public async Task<IActionResult> Category(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var userId = currentUser.Id;

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.PostLikes)
                .Include(p => p.Comments).ThenInclude(c => c.Author)
                .Where(p => p.IsPublished && p.CategoryId == id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.AuthorId,
                UserDisplayName = p.Author.UserName ?? "Unknown User",
                UserProfileImageUrl = p.Author.ProfilePictureUrl,
                LikesCount = p.PostLikes.Count,
                CommentsCount = p.Comments.Count,
                CategoryName = p.Category?.Name ?? "Other",
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                ImageUrl = p.ImageUrl,
                IsLikedByCurrentUser = p.PostLikes.Any(pl => pl.UserId == userId),
                Comments = p.Comments.Select(c => new CommentViewModel
                {
                    Content = c.Content,
                    AuthorName = c.Author.UserName ?? "User",
                    AuthorProfileImageUrl = c.Author.ProfilePictureUrl,
                    CreatedAt = c.CreatedAt
                }).ToList()
            }).ToList();

            var trendingTags = await _context.Tags
                .Select(tag => new
                {
                    tag.Name,
                    PostCount = tag.PostTags.Count()
                })
                .OrderByDescending(t => t.PostCount)
                .Take(5)
                .ToListAsync();

            var trendingTagVMs = trendingTags
                .Select((t, i) => new TrendingTagViewModel
                {
                    Rank = i + 1,
                    Name = t.Name,
                    PostCount = t.PostCount
                }).ToList();

            var categories = await _context.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            var feedViewModel = new FeedViewModel
            {
                CurrentUser = currentUser,
                Posts = postViewModels,
                NewPost = new CreatePostViewModel(),
                TrendingTags = trendingTagVMs,
                Categories = categories
            };

            return View("Main", feedViewModel);  
        }


        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> ToggleLike(int postId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return Unauthorized();

        //    var existingLike = await _context.PostLikes
        //        .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == user.Id);

        //    if (existingLike != null)
        //    {
        //        _context.PostLikes.Remove(existingLike);
        //    }
        //    else
        //    {
        //        _context.PostLikes.Add(new PostLike { PostId = postId, UserId = user.Id });
        //    }

        //    await _context.SaveChangesAsync();

        //    var likeCount = await _context.PostLikes.CountAsync(pl => pl.PostId == postId);
        //    return Json(new { likeCount });
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var existingLike = await _context.PostLikes
                    .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

                bool isLiked;

                if (existingLike != null)
                {
                    _context.PostLikes.Remove(existingLike);
                    isLiked = false;
                }
                else
                {
                    var like = new PostLike
                    {
                        PostId = postId,
                        UserId = userId

                    };
                    _context.PostLikes.Add(like);
                    isLiked = true;
                }

                await _context.SaveChangesAsync();

                var likeCount = await _context.PostLikes
                    .CountAsync(pl => pl.PostId == postId);

                return Json(new
                {
                    likeCount = likeCount,
                    isLiked = isLiked
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Comment cannot be empty.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Comment
            {
                Content = content,
                PostId = postId,
                AuthorId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            var viewModel = new CommentViewModel
            {
                Content = comment.Content,
                AuthorName = user?.UserName ?? "User",
                CreatedAt = comment.CreatedAt
            };

            return PartialView("_SingleComment", viewModel);  
        }





        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            _logger.LogInformation("CreatePost method called"); 
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");
            _logger.LogInformation($"Content received: '{model.Content}'");
            _logger.LogInformation($"CategoryId received: {model.CategoryId}");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }


            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError($"Validation error: {error.ErrorMessage}");
                }
                return RedirectToAction("Main");
            }

            

            if (!ModelState.IsValid)
            {
                var posts = await _context.Posts
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Include(p => p.Comments)
                    .Include(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                    .Where(p => p.IsPublished)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(20)
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
                    Posts = postViewModels,
                    NewPost = model 
                };

                return View("Main", feedViewModel);

            }


            string imagePath = null;

            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);  

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + uniqueFileName;
            }



            var post = new Post
            {
                Subject = model.Content.Length > 50 ? model.Content.Substring(0, 50) + "..." : model.Content,
                Content = model.Content,
                AuthorId = currentUser.Id,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.UtcNow,
                IsPublished = true,
                ImageUrl = imagePath
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(model.Tags))
            {
                //var tagNames = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                //    .Select(t => t.Trim())
                //    .Where(t => !string.IsNullOrEmpty(t))
                //    .ToList();

                var tagNames = model.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t) && t.StartsWith("#"))
                    .Select(t => t.Substring(1)) // no # symbol
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
            _logger.LogInformation("Post created successfully");
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


        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;

                System.Diagnostics.Debug.WriteLine($"User ID: {userId}");
                System.Diagnostics.Debug.WriteLine($"User Name: {userName}");

                if (string.IsNullOrEmpty(userId))
                {
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userByName = await _context.Users
                            .FirstOrDefaultAsync(u => u.UserName == userName);
                        userId = userByName?.Id;
                    }

                    if (string.IsNullOrEmpty(userId))
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not found in database");
                    return RedirectToAction("Login", "Account");
                }

                System.Diagnostics.Debug.WriteLine($"Found user: {user.UserName}");

                var posts = await _context.Posts
                    .Where(p => p.AuthorId == userId)
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(p => p.PostTags)
                    .Include(p => p.PostLikes)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"Found {posts.Count} posts");

                var model = new MyPostsViewModel
                {
                    CurrentUser = user,
                    Posts = posts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MyPosts: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

          
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }
        }


        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.PostLikes)
                .Include(p => p.Comments).ThenInclude(c => c.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.AuthorId,
                UserDisplayName = p.Author.UserName ?? "Unknown User",
                UserProfileImageUrl = p.Author.ProfilePictureUrl,
                LikesCount = p.PostLikes.Count,
                CommentsCount = p.Comments.Count,
                CategoryName = p.Category?.Name ?? "Other",
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                ImageUrl = p.ImageUrl,
                IsLikedByCurrentUser = false, // 
                Comments = p.Comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentViewModel
                {
                    Content = c.Content,
                    AuthorName = c.Author.UserName ?? "User",
                    AuthorProfileImageUrl = c.Author.ProfilePictureUrl,
                    CreatedAt = c.CreatedAt
                }).ToList()
            }).ToList();

            var model = new HomeIndexViewModel
            {
                Posts = postViewModels
            };

            return View(model);
        }


    }


}