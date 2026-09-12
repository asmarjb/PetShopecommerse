using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .OrderByDescending(b => b.PublishedAt)
                .ToListAsync();
            return View(posts);
        }

        public IActionResult Create()
        {
            return View(new BlogPost { PublishedAt = DateTime.UtcNow, IsPublished = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost post, IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(post.Slug))
            {
                post.Slug = (post.Title ?? "post").ToLower().Replace(" ", "-");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                post.ImageUrl = "/uploads/" + fileName;
            }

            post.PublishedAt = DateTime.UtcNow;
            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogPost post, IFormFile? imageFile)
        {
            var existing = await _context.BlogPosts.FindAsync(post.Id);
            if (existing == null) return NotFound();

            existing.Title = post.Title;
            existing.Slug = post.Slug;
            existing.Author = post.Author;
            existing.Summary = post.Summary;
            existing.Content = post.Content;
            existing.IsPublished = post.IsPublished;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existing.ImageUrl = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
