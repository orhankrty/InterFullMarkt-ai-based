using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InterFullMarkt.WebUI.Controllers;

public class AuthController : Controller
{
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