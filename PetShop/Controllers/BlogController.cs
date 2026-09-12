using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.DataContext.Entites;

namespace PetShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Blog
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.PublishedAt)
                .ToListAsync();

            return View(posts);
        }

        // GET: /Blog/Detail/{slug}
        public async Task<IActionResult> Detail(string slug)
        {
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

            if (post == null)
                return NotFound();

            // Recent posts for sidebar
            ViewBag.RecentPosts = await _context.BlogPosts
                .Where(b => b.IsPublished && b.Id != post.Id)
                .OrderByDescending(b => b.PublishedAt)
                .Take(3)
                .ToListAsync();

            return View(post);
        }
    }
}
