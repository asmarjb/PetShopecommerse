using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ParentCategories = new SelectList(
                await _context.ProductCategories.Where(c => c.ParentCategoryId == null).ToListAsync(),
                "Id",
                "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategory category, IFormFile? imageFile)
        {
            // Remove navigation properties and auto-calculated fields from validation
            ModelState.Remove(nameof(ProductCategory.ParentCategory));
            ModelState.Remove(nameof(ProductCategory.SubCategories));
            ModelState.Remove(nameof(ProductCategory.Products));
            ModelState.Remove(nameof(ProductCategory.ImageUrl));
            ModelState.Remove(nameof(ProductCategory.Slug));

            // Generate or clean slug
            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
            }
            else
            {
                category.Slug = GenerateSlug(category.Slug);
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Kateqoriya adı mütləq daxil edilməlidir.");
            }

            // Ensure unique slug
            if (!string.IsNullOrWhiteSpace(category.Slug))
            {
                string baseSlug = category.Slug;
                int counter = 1;
                while (await _context.ProductCategories.AnyAsync(c => c.Slug == category.Slug))
                {
                    category.Slug = $"{baseSlug}-{counter++}";
                }
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    category.ImageUrl = "/uploads/categories/" + uniqueFileName;
                }

                _context.ProductCategories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentCategories = new SelectList(
                await _context.ProductCategories.Where(c => c.ParentCategoryId == null).ToListAsync(),
                "Id",
                "Name",
                category.ParentCategoryId);
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();

            ViewBag.ParentCategories = new SelectList(
                await _context.ProductCategories.Where(c => c.ParentCategoryId == null && c.Id != id).ToListAsync(),
                "Id",
                "Name",
                category.ParentCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory category, IFormFile? imageFile)
        {
            if (id != category.Id) return NotFound();

            ModelState.Remove(nameof(ProductCategory.ParentCategory));
            ModelState.Remove(nameof(ProductCategory.SubCategories));
            ModelState.Remove(nameof(ProductCategory.Products));
            ModelState.Remove(nameof(ProductCategory.ImageUrl));
            ModelState.Remove(nameof(ProductCategory.Slug));

            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
            }
            else
            {
                category.Slug = GenerateSlug(category.Slug);
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Kateqoriya adı mütləq daxil edilməlidir.");
            }

            // Ensure unique slug (excluding current category)
            if (!string.IsNullOrWhiteSpace(category.Slug))
            {
                string baseSlug = category.Slug;
                int counter = 1;
                while (await _context.ProductCategories.AnyAsync(c => c.Slug == category.Slug && c.Id != id))
                {
                    category.Slug = $"{baseSlug}-{counter++}";
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingCategory == null) return NotFound();

                    category.ImageUrl = existingCategory.ImageUrl; // preserve existing if not updated

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        category.ImageUrl = "/uploads/categories/" + uniqueFileName;
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentCategories = new SelectList(
                await _context.ProductCategories.Where(c => c.ParentCategoryId == null && c.Id != id).ToListAsync(),
                "Id",
                "Name",
                category.ParentCategoryId);
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.ProductCategories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                if (category.Products.Any() || category.SubCategories.Any())
                {
                    TempData["ErrorMessage"] = "Bu kateqoriyaya aid məhsul və ya alt-kateqoriyalar olduğu üçün onu silmək mümkün deyil.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.Id == id);
        }

        private static string GenerateSlug(string? phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return Guid.NewGuid().ToString("n")[..8];

            string str = phrase.ToLowerInvariant().Trim();
            // Azerbaijani character replacements
            str = str.Replace("ə", "e")
                     .Replace("ı", "i")
                     .Replace("ö", "o")
                     .Replace("ğ", "g")
                     .Replace("ç", "c")
                     .Replace("ş", "s")
                     .Replace("ü", "u");

            // Remove invalid characters
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Convert multiple spaces/hyphens into one hyphen
            str = Regex.Replace(str, @"[\s-]+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(str) ? Guid.NewGuid().ToString("n")[..8] : str;
        }
    }
}

