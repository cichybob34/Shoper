using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoper.Data;
using Shoper.Models;

namespace Shoper.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        [Authorize]
        public async Task<IActionResult> Index()
        {
              return _context.Product != null ? 
                          View(await _context.Product.Include(p => p.ShoppingList).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Product'  is null.");
        }

        // GET: Products/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["lists"] = GetUserLists();

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,IsPurchased,ShoppingList")] Product product)
        {
            ViewData["lists"] = GetUserLists();

            if (ModelState.IsValid)
            {
                product.ShoppingList = _context.ShoppingList.FirstOrDefault(s => s.Id == Int32.Parse(Request.Form["ShoppingList"]));
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var product = await _context
                .Product
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null || product.ShoppingList!.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }
            
            ViewData["Lists"] = new SelectList(GetUserLists(), nameof(ShoppingList.Id), nameof(ShoppingList.Name), product.ShoppingList.Id);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,IsPurchased,ShoppingList")] Product product)
        {
            var originalProduct = await _context
                .Product
                .AsNoTracking()
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (originalProduct == null || originalProduct.ShoppingList!.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                product.ShoppingList = _context.ShoppingList.FirstOrDefault(s => s.Id == Int32.Parse(Request.Form["ShoppingListId"]));
                _context.Update(product);
                await _context.SaveChangesAsync();
              
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = await _context
                .Product
                .AsNoTracking()
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.ShoppingList!.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context
                .Product
                .AsNoTracking()
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.ShoppingList!.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }
            
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private ICollection<ShoppingList> GetUserLists()
        {
            var user = _userManager
                .Users
                .Include(u => u.ShoppingLists)
                .FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

            return user?.ShoppingLists ?? new List<ShoppingList>();
        }
        
        public async Task<IActionResult> ToggleStatus(int id, string context = "products")
        {
            var product = await _context
                .Product
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == id)!;;
            
            if (product == null || product.ShoppingList!.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
            {
                return NotFound();
            }
            
            product.IsPurchased = !product.IsPurchased;
            _context.Product.Update(product);
            await _context.SaveChangesAsync();

            return context == "shopping-list" ? 
                RedirectToAction("Index", "ShoppingListsProducts", new { id = product.ShoppingList.Id }) : 
                RedirectToAction("Index");
        }
    }
}
