using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterFullMarkt.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.DTOs;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.ValueObjects;
using System.Text.Json;

namespace InterFullMarkt.WebUI.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly IDbContext _dbContext;
    private const string CartSessionKey = "UserCart";

    public CheckoutController(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("/Checkout")]
    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        if (cart.Items.Count == 0) return RedirectToAction("Index", "Store");

        var username = User.Identity?.Name;
        var user = await _dbContext.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return RedirectToAction("Logout", "Auth");

        ViewBag.Cart = cart;
        return View(user);
    }

    [HttpPost("/Checkout/Process")]
    public async Task<IActionResult> Process(string firstName, string lastName, string addressLine, string city, string phoneNumber)
    {
        var cart = GetCart();
        if (cart.Items.Count == 0) return RedirectToAction("Index", "Store");

        var username = User.Identity?.Name;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return RedirectToAction("Logout", "Auth");

        // 1. Create Order
        var shippingAddressStr = $"{firstName} {lastName}, {addressLine}, {city}, Tel: {phoneNumber}";
        var order = new Order(user.Id, Money.Create(cart.TotalAmount, cart.Currency), shippingAddressStr)
        {
            CreatedByUserId = user.Id.ToString()
        };

        // 2. Add Order Items
        foreach (var item in cart.Items)
        {
            var orderItem = new OrderItem(order.Id, item.ProductId, item.Quantity, Money.Create(item.UnitPrice, item.Currency));
            order.OrderItems.Add(orderItem);
        }

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // 3. Clear Cart
        HttpContext.Session.Remove(CartSessionKey);

        TempData["Success"] = "Siparişiniz başarıyla alındı!";
        return RedirectToAction("OrderDetail", "Checkout", new { id = order.Id });
    }

    [HttpGet("/Checkout/Success/{id}")]
    public async Task<IActionResult> OrderDetail(Guid id)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }

    private CartDto GetCart()
    {
        var sessionData = HttpContext.Session.GetString(CartSessionKey);
        return sessionData == null ? new CartDto() : JsonSerializer.Deserialize<CartDto>(sessionData)!;
    }
}
