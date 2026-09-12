using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Areas.Admin.Models;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using System.Security.Claims;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<AdminUser> _hasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<AdminUser>();
        }

        // ─── Register ────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var authResult = await HttpContext.AuthenticateAsync("AdminAuth");
            if (authResult.Succeeded && authResult.Principal.IsInRole("Admin"))
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check for duplicate email (case-insensitive)
            bool emailExists = await _context.AdminUsers
                .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new AdminUser
            {
                Email    = model.Email.Trim().ToLower(),
                FullName = model.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            _context.AdminUsers.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account created successfully. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // ─── Login ────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var authResult = await HttpContext.AuthenticateAsync("AdminAuth");
            if (authResult.Succeeded && authResult.Principal.IsInRole("Admin"))
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.Email == model.Email.Trim().ToLower());

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(model);
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(model);
            }

            // Build claims principal for Admin
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,            user.FullName),
                new Claim(ClaimTypes.Email,           user.Email),
                new Claim(ClaimTypes.Role,            "Admin")
            };

            var identity  = new ClaimsIdentity(claims, "AdminAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc   = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                "AdminAuth",
                principal,
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        // ─── Logout ───────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction(nameof(Login));
        }

        // ─── Access Denied ────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
