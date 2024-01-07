using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shoper.Data;
using Shoper.Models;

namespace Shoper.Controllers
{
    public class ShoppingListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public ShoppingListsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ShoppingLists
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager
                .Users
                .Include(u => u.ShoppingLists)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            return View(user.ShoppingLists);
        }

        // GET: ShoppingLists/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var shoppingList = await _context.ShoppingList
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shoppingList == null || shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ShoppingList shoppingList)
        {
            ModelState.Remove("Owner");
            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                shoppingList.Owner = await _userManager.GetUserAsync(User);
                _context.Add(shoppingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var shoppingList = await _context.ShoppingList.FindAsync(id);

            if (shoppingList == null || shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }

            return View(shoppingList);
        }

        // POST: ShoppingLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,OwnerId")] ShoppingList shoppingList)
        {
            var originalShoppingList = await _context
                .ShoppingList
                .AsNoTracking()
                .FirstOrDefaultAsync(sl => sl.Id == id);

            if (
                originalShoppingList == null ||
                originalShoppingList.Id != id ||
                shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false
            )  {
                return NotFound();
            }

            ModelState.Remove("Owner");
            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                shoppingList.OwnerId = originalShoppingList.OwnerId;
                _context.Update(shoppingList);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var shoppingList = await _context.ShoppingList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingList == null || shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }

            return View(shoppingList);
        }

        // POST: ShoppingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingList = await _context.ShoppingList.FindAsync(id);
            if (shoppingList == null || shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }

            _context.ShoppingList.Remove(shoppingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}