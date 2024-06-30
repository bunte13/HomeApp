using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeApp.Data;
using HomeApp.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeApp.Controllers
{
    public class UserItemsController : Controller
    {
        private readonly HomeAppContext _context;
        private readonly UserManager<HomeAppUser> _userManager;

        public UserItemsController(HomeAppContext context, UserManager<HomeAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserItems
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var name = user.Email;

            var userItems = await _context.UserItem
                .Include(u => u.Item)
                    .ThenInclude(b => b.CategoryItems)
                        .ThenInclude(bg => bg.Category)
                .Include(u => u.Item)
                    .ThenInclude(b => b.Supplier)
                .Where(ub => ub.AppUser == name)
                .ToListAsync();

            return View(userItems);
        }

        // GET: UserItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userItem = await _context.UserItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userItem == null)
            {
                return NotFound();
            }

            return View(userItem);
        }

        // GET: UserItems/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Name");
            return View();
        }

        // POST: UserItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int itemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var appUser = user.Email; // Or use user.Id if you want to store user ID
            var item = await _context.Item.FindAsync(itemId);

            // Check if item exists and if the requested quantity is available
            if (item == null || item.Quantity < quantity)
            {
                // Handle case where item not found or insufficient quantity
                return NotFound(); // Or handle error appropriately
            }

            // Decrease the quantity of the item
            item.Quantity -= quantity;

            UserItem userItem = new UserItem
            {
                AppUser = appUser,
                ItemId = itemId,
                Quantity = quantity
            };

            if (ModelState.IsValid)
            {
                _context.UserItem.Add(userItem);
                _context.Item.Update(item); // Update the item's quantity
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Items");
            }

            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Name", userItem.ItemId);
            return View(userItem);
        }

        // GET: UserItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userItem = await _context.UserItem.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Title", userItem.ItemId);
            return View(userItem);
        }

        // POST: UserItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUser,ItemId")] UserItem userItem)
        {
            if (id != userItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserItemExists(userItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Name", userItem.ItemId);
            return View(userItem);
        }

        // GET: UserItems/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userItem = await _context.UserItem.Include(ui => ui.Item).FirstOrDefaultAsync(ui => ui.Id == id);

            if (userItem == null)
            {
                return NotFound();
            }

            // Increase the quantity of the item
            userItem.Item.Quantity += userItem.Quantity;

            _context.UserItem.Remove(userItem);
            _context.Item.Update(userItem.Item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: UserItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userItem = await _context.UserItem.FindAsync(id);
            if (userItem != null)
            {
                _context.UserItem.Remove(userItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserItemExists(int id)
        {
            return _context.UserItem.Any(e => e.Id == id);
        }
    }
}
