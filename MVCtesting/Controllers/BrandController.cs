using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Brand
        public IActionResult Index(string searchString, DateTime? searchDate, int page = 1)
        {
            int pageSize = 3;

            var brandsQuery = _context.Brands
                .Include(b => b.CategoryBrands)
                .ThenInclude(cb => cb.Category)
                .AsQueryable();

            // Filter by name if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                brandsQuery = brandsQuery.Where(b => b.Name.Contains(searchString));
            }

            // Filter by date if provided
            if (searchDate.HasValue)
            {
                var start = searchDate.Value.Date;
                var end = start.AddDays(1);
                brandsQuery = brandsQuery.Where(b => b.CreatedAt >= start && b.CreatedAt < end);
            }

            // Calculate pagination
            var totalRecords = brandsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var brands = brandsQuery
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentDate = searchDate?.ToString("yyyy-MM-dd");

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(brands);
        }


        public IActionResult Create()
        {
            ViewBag.CategoryList = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BrandCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();
                return View(model);
            }

            var brand = new Brand
            {
                Name = model.Name,
                CategoryBrands = new List<CategoryBrand>()
            };

            if (model.Id.HasValue)
            {
                brand.CategoryBrands.Add(new CategoryBrand { CategoryId = model.Id.Value });
            }

            _context.Brands.Add(brand);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Brand successfully created.";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Brand/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var brand = _context.Brands
                .Include(b => b.CategoryBrands)
                .ThenInclude(cb => cb.Category)
                .FirstOrDefault(b => b.Id == id);

            if (brand == null)
            {
                TempData["ErrorMessage"] = "Brand not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new BrandCreateViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                SelectedCategoryIds = brand.CategoryBrands.Select(cb => cb.CategoryId).ToList(), // Currently selected IDs
                CategoriesSelectList = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        // POST: /Brand/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BrandCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate CategoriesSelectList if validation fails
                model.CategoriesSelectList = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            var brand = _context.Brands
                .Include(b => b.CategoryBrands)
                .FirstOrDefault(b => b.Id == model.Id);

            if (brand == null)
            {
                TempData["ErrorMessage"] = "Brand not found.";
                return RedirectToAction(nameof(Index));

            }

            // Update properties
            brand.Name = model.Name;

            // Update Categories
            // Remove existing ones first
            _context.CategoryBrands.RemoveRange(brand.CategoryBrands);

            // Then add the new ones
            if (model.SelectedCategoryIds != null && model.SelectedCategoryIds.Any())
            {
                foreach (var categoryId in model.SelectedCategoryIds)
                {
                    _context.CategoryBrands.Add(new CategoryBrand
                    {
                        BrandId = brand.Id,
                        CategoryId = categoryId
                    });
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Brand updated successfully.";
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var brand = _context.Brands
                .Include(b => b.CategoryBrands)
                .FirstOrDefault(b => b.Id == id);

            if (brand == null)
            {
                TempData["ErrorMessage"] = "Brand not found.";
                return RedirectToAction(nameof(Index));

            }

            if (brand.CategoryBrands.Any())
            {
                _context.CategoryBrands.RemoveRange(brand.CategoryBrands);
            }

            _context.Brands.Remove(brand);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Brand deleted successfully.";
            return RedirectToAction(nameof(Index));

        }
    }
}
