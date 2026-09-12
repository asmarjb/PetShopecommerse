using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.AdminUsers
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            
            return View(users);
        }

        public IActionResult Create()
        {
            return View(new PetShop.Areas.Admin.Models.AddUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetShop.Areas.Admin.Models.AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.AdminUsers.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(model);
            }

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<AdminUser>();
            var newUser = new AdminUser
            {
                FullName = model.FullName,
                Email = model.Email,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            
            newUser.PasswordHash = hasher.HashPassword(newUser, model.Password);

            _context.AdminUsers.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Prevent deleting yourself
            var currentUserIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.AdminUsers.FindAsync(id);
            if (user == null) return NotFound();

            _context.AdminUsers.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User '{user.FullName}' has been deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

