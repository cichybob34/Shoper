using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoper.Data;
using Shoper.Models;
using Microsoft.AspNetCore.Authorization;

public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var statistics = new Statistics
        {
            TotalProducts = await _context.Product.CountAsync(),
            TotalLists = await _context.ShoppingList.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
        };

        return View(statistics);
    }
}