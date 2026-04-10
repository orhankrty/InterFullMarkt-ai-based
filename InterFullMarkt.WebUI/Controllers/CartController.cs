using Microsoft.AspNetCore.Mvc;
using InterFullMarkt.Application.DTOs;
using InterFullMarkt.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InterFullMarkt.WebUI.Controllers;

public class CartController : Controller
{
    private readonly IDbContext _dbContext;
    private const string CartSessionKey = "UserCart";

    public CartController(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return NotFound();

        var cart = GetCart();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price.Amount,
                Currency = product.Price.Currency,
                ImageUrl = product.ImageUrl,
                Quantity = 1
            });
        }

        SaveCart(cart);
        return Json(new { success = true, cartCount = cart.TotalQuantity, cartTotal = cart.TotalAmount + " " + cart.Currency });
    }

    [HttpGet]
    public IActionResult GetCartSummary()
    {
        var cart = GetCart();
        return Json(cart);
    }

    [HttpPost]
    public IActionResult RemoveFromCart(Guid productId)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCart(cart);
        }
        return Json(new { success = true, cartCount = cart.TotalQuantity });
    }

    [HttpPost]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return Json(new { success = true });
    }

    private CartDto GetCart()
    {
        var sessionData = HttpContext.Session.GetString(CartSessionKey);
        return sessionData == null ? new CartDto() : JsonSerializer.Deserialize<CartDto>(sessionData)!;
    }

    private void SaveCart(CartDto cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }
}
