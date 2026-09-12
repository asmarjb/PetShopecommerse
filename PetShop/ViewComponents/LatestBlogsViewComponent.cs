using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;

namespace PetShop.ViewComponents
{
    public class LatestBlogsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public LatestBlogsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 3)
        {
            var posts = await _context.BlogPosts
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.PublishedAt)
                .Take(count)
                .ToListAsync();

            return View(posts);
        }
    }
}
