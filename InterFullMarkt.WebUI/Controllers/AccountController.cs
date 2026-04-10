using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterFullMarkt.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.DTOs;
using System.Security.Claims;

namespace InterFullMarkt.WebUI.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IDbContext _dbContext;

    public AccountController(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users
            .Include(u => u.Orders)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return NotFound();

        return View(user);
    }

    [Route("Account/Orders")]
    public async Task<IActionResult> Orders()
    {
        var username = User.Identity?.Name;
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.User!.Username == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [Route("Account/Profile")]
    public async Task<IActionResult> Profile()
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        return View(user);
    }

    [HttpPost]
    [Route("Account/Profile")]
    public async Task<IActionResult> Profile(string firstName, string lastName, string phoneNumber)
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user != null)
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            TempData["Success"] = "Profil başarıyla güncellendi.";
        }
        
        return RedirectToAction(nameof(Profile));
    }
}
