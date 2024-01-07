using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoper.Data;
using Shoper.Models;

namespace Shoper.Controllers;

public class ShoppingListsProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ShoppingListsProductsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    // GET: ShoppingLists/Products/5
    [Authorize]
    [HttpGet("ShoppingLists/{id:int}/Products")]
    public async Task<IActionResult> Index(int id)
    {
        var shoppingList = await _context.ShoppingList
            .Include(l => l.Products)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (shoppingList == null || shoppingList.HasAccess(_userManager.GetUserId(HttpContext.User)) == false)
        {
            return NotFound();
        }

        return View("Index", shoppingList);
    }

    // GET: ShoppingLists/Products/Create/{id}
    [Authorize]
    [HttpGet("ShoppingLists/{id:int}/Products/Create")]
    public async Task<IActionResult> Create(int id)
    {
        var shoppingList = await _context.ShoppingList
            .FirstOrDefaultAsync(m => m.Id == id);

        if (shoppingList == null)
        {
            return NotFound();
        }

        ViewData["listName"] = shoppingList.Name;

        return View("Create");
    }

    // Post
    [Authorize]
    [HttpPost("ShoppingLists/{id:int}/Products/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, [Bind("Name,Quantity,Price,IsPurchased")] Product product)
    {
        if (ModelState.IsValid)
        {
            product.ShoppingList = _context.ShoppingList.FirstOrDefault(s => s.Id == id);
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = id });
        }

        return View("Create", product);
    }
}