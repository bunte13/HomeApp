using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeApp.Data;
using HomeApp.Models;
using HomeApp.ViewModel;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Identity;

namespace HomeApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly HomeAppContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<HomeAppUser> _userManager;

        public ItemsController(HomeAppContext context, IWebHostEnvironment environment, UserManager<HomeAppUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
            
        }
        public IActionResult ViewFile(string fileName)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (System.IO.File.Exists(filePath))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(fileName, out var contentType))
                {
                    return PhysicalFile(filePath, contentType);
                }
                else
                {
                    return PhysicalFile(filePath, "application/octet-stream"); // Default to octet-stream if MIME type cannot be determined
                }
            }
            else
            {
                return NotFound(); // Return 404 if the file does not exist
            }
        }
        // GET: Items
        public async Task<IActionResult> Index(string CategoryItem, string searchString, string SupplierSearchString)
        {
            var categoryQuery = _context.Category.OrderBy(c => c.CatgoryName).Select(c => c.CatgoryName).Distinct();
            var itemQuery = _context.Item
                .Include(b => b.userItems)
                .Include(b => b.Reviews)
                .Include(b => b.CategoryItems)
                .ThenInclude(bg => bg.Category)
                .Include(b => b.Supplier)
                .AsQueryable();
            var SuppliersQuery = _context.Supplier.AsQueryable();

            if (!string.IsNullOrEmpty(CategoryItem))
            {
                itemQuery = itemQuery.Where(b => b.CategoryItems.Any(bg => bg.Category.CatgoryName == CategoryItem));
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                itemQuery = itemQuery.Where(b => b.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(SupplierSearchString))
            {
                itemQuery = itemQuery.Where(a => (a.Supplier.Name).Contains(SupplierSearchString));
            }

            var categories = await categoryQuery.ToListAsync();
            var items = await itemQuery.ToListAsync();
            var suppliers = await SuppliersQuery.ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            var userItems = new List<int>();

            if (currentUser != null)
            {
                userItems = await _context.UserItem
                    .Where(ui => ui.AppUser == currentUser.UserName)
                    .Select(ui => ui.ItemId)
                    .ToListAsync();
            }
            

            var viewModel = new ItemCategoryViewModel
            {
                items = items,
                Categories = new SelectList(categories),
                CategoryItem = CategoryItem,
                SearchString = searchString,
                Suppliers = suppliers,
                SupplierSearchString = SupplierSearchString,
                UserItems = userItems

            };

            return View(viewModel);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(r=>r.Reviews)
                .Include(r=>r.Supplier)
                .Include(c => c.CategoryItems)
                .ThenInclude(c=>c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isOwnedByCurrentUser = user != null && _context.UserItem.Any(ui => ui.AppUser == user.UserName && ui.ItemId == item.Id);

            var viewModel = new ItemDetailViewModel
            {
                Item = item,
                IsOwnedByCurrentUser = isOwnedByCurrentUser
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var categories = _context.Category.ToList();
            var suppliers = _context.Supplier.ToList();

            var viewModel = new CategoryItemEditCreateViewModel
            {
                CategoriesList = new MultiSelectList(categories, "Id", "CatgoryName"),
                SuppliersList = new SelectList(suppliers, "Id", "Name")
            };

            return View(viewModel);
        }


        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryItemEditCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.FrontPageFile != null && viewModel.FrontPageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.FrontPageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.FrontPageFile.CopyToAsync(fileStream);
                    }

                    viewModel.Item.PictureJPG = "/uploads/" + uniqueFileName;
                }

                viewModel.Item.SupplierId = viewModel.SelectedSupplierId;

                _context.Add(viewModel.Item);
                await _context.SaveChangesAsync();

                foreach (int item in viewModel.SelectedCategories)
                {
                    _context.CategoryItem.Add(new CategoryItem { CategoryId = item, ItemId = viewModel.Item.Id });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = _context.Category.ToList();
            viewModel.CategoriesList = new MultiSelectList(categories, "Id", "CatgoryName");
            var suppliers = _context.Supplier.ToList();
            viewModel.SuppliersList = new SelectList(suppliers, "Id", "Name");

            return View(viewModel);
        }


        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(m => m.CategoryItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var categories = _context.Category.ToList();
            var suppliers = _context.Supplier.ToList();

            var viewModel = new CategoryItemEditCreateViewModel
            {
                Item = item,
                CategoriesList = new MultiSelectList(categories, "Id", "CatgoryName"),
                SuppliersList = new SelectList(suppliers, "Id", "Name"),
                SelectedCategories = item.CategoryItems.Select(uf => uf.CategoryId).ToList(),
                SelectedSupplierId = item.SupplierId
            };

            return View(viewModel);
        }


        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryItemEditCreateViewModel viewModel)
        {
            if (id != viewModel.Item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.FrontPageFile != null && viewModel.FrontPageFile.Length > 0)
                    {
                        string uniqueFrontPageFileName = Guid.NewGuid().ToString() + "_" + viewModel.FrontPageFile.FileName;
                        string frontPageFilePath = Path.Combine(_environment.WebRootPath, "uploads", uniqueFrontPageFileName);

                        using (var fileStream = new FileStream(frontPageFilePath, FileMode.Create))
                        {
                            await viewModel.FrontPageFile.CopyToAsync(fileStream);
                        }

                        viewModel.Item.PictureJPG = "/uploads/" + uniqueFrontPageFileName;
                    }

                    if (viewModel.FrontPageFile == null)
                    {
                        var existingItem = await _context.Item.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                        if (existingItem != null)
                        {
                            viewModel.Item.PictureJPG = existingItem.PictureJPG;
                        }
                    }

                    viewModel.Item.SupplierId = viewModel.SelectedSupplierId;

                    _context.Update(viewModel.Item);
                    await _context.SaveChangesAsync();

                    var oldCategories = _context.CategoryItem.Where(bg => bg.ItemId == id);
                    _context.CategoryItem.RemoveRange(oldCategories);
                    await _context.SaveChangesAsync();

                    foreach (int catId in viewModel.SelectedCategories)
                    {
                        _context.CategoryItem.Add(new CategoryItem { CategoryId = catId, ItemId = id });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.InnerException?.Message);
                    ModelState.AddModelError("", "An error occurred while updating the item. Please try again later.");
                }

                return RedirectToAction(nameof(Index));
            }

            var categories = _context.Category.ToList();
            viewModel.CategoriesList = new MultiSelectList(categories, "Id", "CatgoryName");
            var suppliers = _context.Supplier.ToList();
            viewModel.SuppliersList = new SelectList(suppliers, "Id", "Name");

            return View(viewModel);
        }


        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            if (item != null)
            {
                _context.Item.Remove(item);
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
