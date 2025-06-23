using CyberHub.Models;
using CyberHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CyberHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment; 

        }

        // regsiter get
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings(bool updated = false)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new SettingsViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                ProfilePictureUrl = user.ProfilePictureUrl
            };
            if (updated)
                ViewBag.SuccessMessage = "Profile updated successfully.";

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            model.ProfilePictureUrl = user.ProfilePictureUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Handle password change first (if provided)
            bool passwordChanged = false;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                Console.WriteLine($"Password change requested for user: {user.UserName}");
                Console.WriteLine($"New password length: {model.NewPassword.Length}");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                Console.WriteLine($"Generated password reset token: {token != null}");

                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                Console.WriteLine($"Password reset result: {passwordResult.Succeeded}");

                if (!passwordResult.Succeeded)
                {
                    Console.WriteLine("Password reset failed with errors:");
                    foreach (var error in passwordResult.Errors)
                    {
                        Console.WriteLine($"Error: {error.Code} - {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                Console.WriteLine("Password changed successfully");
                passwordChanged = true;
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                Console.WriteLine("File name: " + model.ProfilePicture.FileName);
                Console.WriteLine("Content length: " + model.ProfilePicture.Length);

                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && user.ProfilePictureUrl != "/images/default.jpg")
                {
                    var oldPath = Path.Combine(_hostEnvironment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfilePicture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = "/uploads/" + fileName;
                Console.WriteLine("Assigned URL: " + user.ProfilePictureUrl);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            if (passwordChanged)
            {
                Console.WriteLine("Signing out user after password change");
                await _signInManager.SignOutAsync();
                TempData["SuccessMessage"] = "Settings updated successfully. Please log in with your new password.";
                Console.WriteLine("Redirecting to login page");
                return RedirectToAction("Login", "Account");
            }

            Console.WriteLine("No password change, refreshing sign-in");
            await _signInManager.RefreshSignInAsync(user);

            var updatedUser = await _userManager.FindByIdAsync(user.Id);
            Console.WriteLine("Updated URL in DB: " + updatedUser?.ProfilePictureUrl);

            TempData["SuccessMessage"] = "Settings updated successfully.";
            return RedirectToAction("Settings");
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User { UserName = model.UserName, Email = model.Email,  PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Main", "Home");

            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Main", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    

    }
}
