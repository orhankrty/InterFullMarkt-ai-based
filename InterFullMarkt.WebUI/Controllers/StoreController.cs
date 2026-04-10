using Microsoft.AspNetCore.Mvc;
using InterFullMarkt.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace InterFullMarkt.WebUI.Controllers;

public class StoreController : Controller
{
    private readonly IDbContext _dbContext;

    public StoreController(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _dbContext.Products
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.IsFeatured)
            .ToListAsync();

        return View(products);
    }
}