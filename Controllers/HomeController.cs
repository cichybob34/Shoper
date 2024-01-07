using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoper.Data;
using Shoper.Models;
using System.Diagnostics;

namespace Shoper.Controllers
{
	public class HomeController : Controller
	{
        private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}


        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _context.Product
                .Where(p => p.Name.Contains(searchTerm))
            .ToListAsync();

			if (products.Any())
			{
                return RedirectToAction("Details", "Products", new {id = products.First().Id});
            }

            var shoppingLists = await _context.ShoppingList
                .Where(s => s.Name.Contains(searchTerm))
                .ToListAsync();

			if (shoppingLists.Any())
            {
                return RedirectToAction("Details", "ShoppingLists", new { id = shoppingLists.First().Id });
            }

			ViewData["error"] = "Brak wyników wyszukiwania";
			
            return View("Index");
        }
	}
}