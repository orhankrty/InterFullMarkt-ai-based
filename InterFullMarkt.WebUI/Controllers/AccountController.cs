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

        if (user == null) return RedirectToAction("Logout", "Auth");

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
        if (user == null) return RedirectToAction("Logout", "Auth");
        return View(user);
    }

    [HttpPost]
    [Route("Account/Profile")]
    public async Task<IActionResult> Profile(string firstName, string lastName, string phoneNumber)
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Logout", "Auth");

        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        TempData["Success"] = "Profil başarıyla güncellendi.";
        
        return RedirectToAction(nameof(Profile));
    }

    [Route("Account/Addresses")]
    public async Task<IActionResult> Addresses()
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Logout", "Auth");
        
        return View(user);
    }

    [HttpPost]
    [Route("Account/AddAddress")]
    public async Task<IActionResult> AddAddress(string title, string city, string district, string addressLine)
    {
        var username = User.Identity?.Name;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Logout", "Auth");

        var address = new InterFullMarkt.Domain.Entities.Address(user.Id, title, user.FirstName ?? username, user.LastName ?? "User", user.PhoneNumber ?? "0000000000", city, district, addressLine)
        {
            CreatedByUserId = user.Id.ToString()
        };
        
        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        
        TempData["Success"] = "Yeni adresiniz başarıyla eklendi.";
        return RedirectToAction(nameof(Addresses));
    }

    [HttpPost]
    [Route("Account/DeleteAddress/{id}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == id);
        if (address != null)
        {
            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            TempData["Success"] = "Adres başarıyla silindi.";
        }
        return RedirectToAction(nameof(Addresses));
    }
}
