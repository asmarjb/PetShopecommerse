using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var user = await _context.AdminUsers.FindAsync(userId);
            if (user == null) return NotFound();

            var model = new PetShop.Areas.Admin.Models.ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PetShop.Areas.Admin.Models.ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var user = await _context.AdminUsers.FindAsync(userId);
            if (user == null) return NotFound();

            // Check if email is being changed and already exists
            if (user.Email != model.Email && await _context.AdminUsers.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already in use by another account.");
                return View(model);
            }

            user.FullName = model.FullName;
            user.Email = model.Email;

            if (model.RemoveProfileImage)
            {
                user.ProfileImageUrl = null;
            }
            else if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(fileStream);
                }
                user.ProfileImageUrl = "/uploads/avatars/" + uniqueFileName;
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to set a new password.");
                    model.ProfileImageUrl = user.ProfileImageUrl; // retain image on error
                    return View(model);
                }

                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<AdminUser>();
                
                // Verify old password
                var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
                if (verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    model.ProfileImageUrl = user.ProfileImageUrl;
                    return View(model);
                }

                user.PasswordHash = hasher.HashPassword(user, model.NewPassword);
            }

            await _context.SaveChangesAsync();

            // Refresh the cookie so the new name shows in the layout immediately
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var user = await _context.AdminUsers.FindAsync(userId);
            if (user == null) return NotFound();

            user.ProfileImageUrl = null;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

