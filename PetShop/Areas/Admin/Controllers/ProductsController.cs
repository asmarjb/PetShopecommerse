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
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.ProductCategories, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile>? imageFiles)
        {
            ModelState.Remove(nameof(Product.ProductCategory));
            ModelState.Remove(nameof(Product.Brand));
            ModelState.Remove(nameof(Product.Images));
            ModelState.Remove(nameof(Product.Variants));
            ModelState.Remove(nameof(Product.ProductAnimalTypes));

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync(); // save to get product ID

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    bool isFirst = true;
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = "/uploads/products/" + uniqueFileName,
                                IsMain = isFirst
                            });
                            isFirst = false;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.ProductCategories, "Id", "Name", product.ProductCategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.ProductCategories, "Id", "Name", product.ProductCategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile>? imageFiles)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove(nameof(Product.ProductCategory));
            ModelState.Remove(nameof(Product.Brand));
            ModelState.Remove(nameof(Product.Images));
            ModelState.Remove(nameof(Product.Variants));
            ModelState.Remove(nameof(Product.ProductAnimalTypes));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null) return NotFound();
                    
                    existingProduct.Name = product.Name;
                    existingProduct.Sku = product.Sku;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.DiscountPrice = product.DiscountPrice;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.ProductCategoryId = product.ProductCategoryId;
                    existingProduct.BrandId = product.BrandId;
                    existingProduct.IsActive = product.IsActive;

                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
                        Directory.CreateDirectory(uploadsFolder);

                        bool hasMain = existingProduct.Images.Any(i => i.IsMain);

                        foreach (var file in imageFiles)
                        {
                            if (file.Length > 0)
                            {
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }

                                existingProduct.Images.Add(new ProductImage
                                {
                                    ImageUrl = "/uploads/products/" + uniqueFileName,
                                    IsMain = !hasMain
                                });
                                hasMain = true;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.ProductCategories, "Id", "Name", product.ProductCategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
                
                // If it was main, make another image main
                if (image.IsMain)
                {
                    var newMain = await _context.ProductImages.FirstOrDefaultAsync(i => i.ProductId == image.ProductId);
                    if (newMain != null)
                    {
                        newMain.IsMain = true;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Ok();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                // Set all other images for this product to not main
                var allImages = await _context.ProductImages.Where(i => i.ProductId == image.ProductId).ToListAsync();
                foreach(var img in allImages)
                {
                    img.IsMain = false;
                }
                
                // Set this image to main
                image.IsMain = true;
                
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

