using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCtesting.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using MVCtesting.Services;

namespace MVCtesting.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }



        public IActionResult Index(string searchString, DateTime? searchDate, int page = 1)
        {
            int pageSize = 3;

            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            if (searchDate.HasValue)
            {
                var date = searchDate.Value.Date;
                query = query.Where(c => EF.Functions.DateDiffDay(c.CreatedAt, date) == 0);
            }


            int totalItems = query.Count();

            var categories = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentDate = searchDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(categories);
        }








        // GET: /Category/Create
        // GET: /Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CategoryCreateViewModel();

            // Populate BrandsSelectList for multi-select
            model.BrandsSelectList = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToList();

            return View(model);
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the BrandsSelectList on validation error
                model.BrandsSelectList = _context.Brands
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    })
                    .ToList();

                return View(model);
            }

            // Create new Category entity
            var category = new Category
            {
                Name = model.Name,
                CategoryBrands = new List<CategoryBrand>()
            };

            // Associate with selected Brands if any
            if (model.SelectedBrandIds != null && model.SelectedBrandIds.Any())
            {
                foreach (var brandId in model.SelectedBrandIds)
                {
                    category.CategoryBrands.Add(new CategoryBrand
                    {
                        BrandId = brandId
                    });
                }
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category successfully created.";
            return RedirectToAction(nameof(Index)); // Redirect back to Index after creating
        }
        // GET: /Category/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories
                .Include(c => c.CategoryBrands)
                .ThenInclude(cb => cb.Brand)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryCreateViewModel
            {
                Id = category.Id,
                Name = category.Name,
                SelectedBrandIds = category.CategoryBrands.Select(cb => cb.BrandId).ToList(), // Currently selected IDs
                BrandsSelectList = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList()
            };

            return View(model);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate BrandsSelectList if validation fails
                model.BrandsSelectList = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();

                return View(model);
            }

            var category = _context.Categories
                .Include(c => c.CategoryBrands)
                .FirstOrDefault(c => c.Id == model.Id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            // Update properties
            category.Name = model.Name;

            // Update Brands
            // Remove existing ones first
            _context.CategoryBrands.RemoveRange(category.CategoryBrands);

            // Then add the new ones
            if (model.SelectedBrandIds != null && model.SelectedBrandIds.Any())
            {
                foreach (var brandId in model.SelectedBrandIds)
                {
                    _context.CategoryBrands.Add(new CategoryBrand
                    {
                        CategoryId = category.Id,
                        BrandId = brandId
                    });
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                .Include(c => c.CategoryBrands)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            if (category.CategoryBrands != null && category.CategoryBrands.Any())
            {
                _context.CategoryBrands.RemoveRange(category.CategoryBrands);
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }



    }
}