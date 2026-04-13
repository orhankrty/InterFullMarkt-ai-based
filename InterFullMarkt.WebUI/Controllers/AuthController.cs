using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterFullMarkt.WebUI.Controllers;

public class AuthController : Controller
{
    private readonly IDbContext _dbContext;

    public AuthController(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Eğer kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Players");
            
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Mock (Sahte) Giriş İşlemi - İleride veritabanı (Identity) ile değiştirilecek
        if (username == "admin" && password == "123456")
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                user = new User(username, "admin@interfullmarkt.com", "hashed_pw", "Admin")
                {
                    FirstName = "System",
                    LastName = "Admin",
                    CreatedByUserId = "System"
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync(CancellationToken.None);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Players");
        }

        ViewData["Error"] = "Kullanıcı adı veya şifre hatalı!";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Players");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
        {
            ViewData["Error"] = "Lütfen tüm alanları doldurun!";
            return View();
        }

        // Mock (Sahte) Kayıt ve Giriş İşlemi - İleride veritabanı eklenecek
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new User(username, email, "hashed_pw", "User")
            {
                FirstName = username,
                LastName = "User",
                CreatedByUserId = "System"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "User")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity));

        TempData["Success"] = "Kayıt başarılı! Sisteme hoş geldiniz.";
        return RedirectToAction("Index", "Players");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Players");
    }
}