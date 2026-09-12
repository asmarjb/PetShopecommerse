using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;
using PetShop.Models;

namespace PetShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<AppUser> _hasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<AppUser>();
        }

        // ==========================================
        // LOGIN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var authResult = await HttpContext.AuthenticateAsync("UserAuth");
            if (authResult.Succeeded && authResult.Principal.IsInRole("User"))
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(Profile));
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new UserLoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var normalizedEmail = model.Email.Trim().ToLower();
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Daxil edilmiş e-poçt və ya şifrə yanlışdır.");
                return View(model);
            }

            var verifyResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Daxil edilmiş e-poçt və ya şifrə yanlışdır.");
                return View(model);
            }

            // Create Claims for Customer
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            if (!string.IsNullOrEmpty(user.Phone))
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, user.Phone));
            }

            var identity = new ClaimsIdentity(claims, "UserAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)
                    : DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync("UserAuth", principal, authProps);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // REGISTER
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var authResult = await HttpContext.AuthenticateAsync("UserAuth");
            if (authResult.Succeeded && authResult.Principal.IsInRole("User"))
                return RedirectToAction(nameof(Profile));

            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var normalizedEmail = model.Email.Trim().ToLower();
            bool emailExists = await _context.AppUsers
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Bu e-poçt ünvanı ilə artıq qeydiyyatdan keçilib.");
                return View(model);
            }

            var user = new AppUser
            {
                FullName = model.FullName.Trim(),
                Email = normalizedEmail,
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            // Auto-login after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };
            if (!string.IsNullOrEmpty(user.Phone))
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, user.Phone));
            }

            var identity = new ClaimsIdentity(claims, "UserAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
            };

            await HttpContext.SignInAsync("UserAuth", principal, authProps);

            TempData["SuccessMessage"] = "Qeydiyyat uğurla tamamlandı! Xoş gəlmisiniz.";
            return RedirectToAction(nameof(Profile));
        }

        // ==========================================
        // LOGOUT
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "UserAuth")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserAuth");
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // PROFILE & ORDERS
        // ==========================================
        [HttpGet]
        [Authorize(AuthenticationSchemes = "UserAuth", Roles = "User")]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction(nameof(Login));

            var user = await _context.AppUsers.FindAsync(userId);
            if (user == null || !user.IsActive)
            {
                await HttpContext.SignOutAsync("UserAuth");
                return RedirectToAction(nameof(Login));
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                City = user.City,
                CreatedAt = user.CreatedAt,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "UserAuth", Roles = "User")]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction(nameof(Login));

            var user = await _context.AppUsers.FindAsync(userId);
            if (user == null || !user.IsActive)
            {
                await HttpContext.SignOutAsync("UserAuth");
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid)
            {
                // Reload orders for the view
                model.Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                return View(nameof(Profile), model);
            }

            user.FullName = model.FullName.Trim();
            user.Phone = model.Phone?.Trim();
            user.Address = model.Address?.Trim();
            user.City = model.City?.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profil məlumatlarınız uğurla yeniləndi.";
            return RedirectToAction(nameof(Profile));
        }

        // ==========================================
        // ACCESS DENIED
        // ==========================================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
