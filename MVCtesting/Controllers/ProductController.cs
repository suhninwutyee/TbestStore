using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using Microsoft.AspNetCore.Authorization;
using MVCtesting.Services;


namespace MVCtesting.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment environment;
        private readonly object filteredProducts;

        public ProductController(ApplicationDbContext context ,IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");  // or wherever you want to redirect after logout
        }

        public IActionResult Index(string searchString, DateTime? searchDate, int? categoryId, int page = 1)
        {
            int pageSize = 3;

            var productsQuery = _context.Tproducts
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(searchString) || p.Brand.Name.Contains(searchString));
            }

            if (searchDate.HasValue)
            {
                productsQuery = productsQuery.Where(p =>
                    p.CreatedAt.HasValue && p.CreatedAt.Value.Date == searchDate.Value.Date);
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            // Paging
            int totalProducts = productsQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var pagedProducts = productsQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBags
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentDate = searchDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedProducts);
        }





        public IActionResult Create()
        {
            ViewBag.CategoryId = _context.Categories
            .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
            }).ToList();


            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            // Check if product with the same Name AND Brand already exists
            bool productExists = _context.Tproducts.Any(x => x.Name == productDto.Name && x.BrandId == productDto.BrandId);

            if (productExists)
            {
                ViewBag.ErrorMessage = "A product with the same Name and Brand already exists.";

                // Reload dropdowns so the form can display correctly
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", productDto.CategoryId);

                ViewBag.BrandId = _context.Brands
                    .Join(_context.CategoryBrands,
                        b => b.Id,
                        cb => cb.BrandId,
                        (b, cb) => new { cb.BrandId, cb.CategoryId, b.Name })
                    .Where(b => b.CategoryId == productDto.CategoryId)
                    .Select(b => new SelectListItem
                    {
                        Value = b.BrandId.ToString(),
                        Text = b.Name
                    }).ToList();

                return View(productDto);
            }

            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", productDto.CategoryId);

                ViewBag.BrandId = _context.Brands
                    .Join(_context.CategoryBrands,
                        b => b.Id,
                        cb => cb.BrandId,
                        (b, cb) => new { cb.BrandId, cb.CategoryId, b.Name })
                    .Where(b => b.CategoryId == productDto.CategoryId)
                    .Select(b => new SelectListItem
                    {
                        Value = b.BrandId.ToString(),
                        Text = b.Name
                    }).ToList();

                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            int newproductId = _context.Tproducts.Any() ? _context.Tproducts.Max(t => t.product_id) + 1 : 1;

            Product product = new Product()
            {
                product_id = newproductId,
                Name = productDto.Name,
                BrandId = productDto.BrandId ,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                Description = productDto.Description,
                Quantity = productDto.Quantity,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            _context.Tproducts.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }      
        public IActionResult Edit(int id)
        {
            // Use product_id here instead of Id
            var product = _context.Tproducts
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.product_id == id);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            var productDto = new ProductDto()
            {
                product_id = product.product_id,
                Name = product.Name,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId ?? 0,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity
            };

            ViewData["ProductId"] = product.product_id;
            ViewData["ImageFileName"] = product.ImageFileName ?? "noimage.jpg";
            ViewData["CreatedAt"] = product.CreatedAt?.ToString("MM/dd/yyyy") ?? "";

            ViewBag.CategoryList = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            ViewBag.BrandList = _context.CategoryBrands
            .Include(cb => cb.Brand)
            .Where(cb => cb.CategoryId == product.CategoryId)
            .Select(cb => new SelectListItem
            {
                Value = cb.BrandId.ToString(),
                Text = cb.Brand.Name
            }).ToList();

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = _context.Tproducts.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Clear ModelState for Name to avoid old values sticking around
            ModelState.Remove("Name");
            if (!ModelState.IsValid)
            {
                // Reload dropdowns & data if validation fails
                ViewData["ProductId"] = product.product_id;
                ViewData["ImageFileName"] = product.ImageFileName ?? "noimage.jpg";
                ViewData["CreatedAt"] = product.CreatedAt?.ToString("MM/dd/yyyy") ?? "";

                ViewBag.CategoryList = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                ViewBag.BrandList = _context.CategoryBrands
                    .Include(cb => cb.Brand)
                    .Where(cb => cb.CategoryId == product.CategoryId)
                    .Select(cb => new SelectListItem
                    {
                        Value = cb.BrandId.ToString(),
                        Text = cb.Brand.Name
                    }).ToList();

                return View(productDto);
            }

            // Handle image upload
            string newFileName = product.ImageFileName ?? "noimage.jpg";
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // Delete old image if it exists and is not the default
                string oldImageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
                if (!string.IsNullOrEmpty(product.ImageFileName) && product.ImageFileName != "noimage.jpg" && System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Now update product properties from DTO
            product.Name = productDto.Name;
            product.BrandId = productDto.BrandId ; // Assuming BrandId nullable, otherwise just productDto.BrandId
            product.CategoryId = productDto.CategoryId;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _context.Tproducts.FirstOrDefault(p => p.product_id == id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }

            // Optional: Delete image file
            if (!string.IsNullOrEmpty(product.ImageFileName))
            {
                var imagePath = Path.Combine(environment.WebRootPath, "product", product.ImageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Tproducts.Remove(product);
            _context.SaveChanges();
            var products = _context.Tproducts.OrderBy(p => p.Id).ToList();
            int counter = 1;
            foreach (var p in products)
            {
                p.product_id = counter++;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }

        public JsonResult GetBrandsByCategory(int id)
        {
            var brands = _context.CategoryBrands
                .Where(cb => cb.CategoryId == id)
                .Select(cb => new SelectListItem
                {
                    Value = cb.Brand.Id.ToString(),
                    Text = cb.Brand.Name
                })
                .ToList();

            return Json(brands);
        }
        public IActionResult MakeOrder()
        {
            var products = _context.Tproducts
        .Include(p => p.Category)
        .ToList();
            Console.WriteLine(" product.................................." + products.Count);

            // Group by CategoryId and take first product in each group
            var uniqueProductsByCategory = products
                .GroupBy(p => p.CategoryId)
                .Select(g => g.First())
                .ToList();
            Console.WriteLine(" unique product.................................." + uniqueProductsByCategory.Count);
            return View(uniqueProductsByCategory);
        }
        public IActionResult Details(int id)
        {
            var product = _context.Tproducts
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.product_id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }




    }
}
