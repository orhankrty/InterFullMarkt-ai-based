namespace InterFullMarkt.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;

public class StoreController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        // Mağaza ürünlerinin listesi (Mock Data)
        var products = new List<ProductViewModel>
        {
            new(1, "Profesyonel Krampon 'SpeedX'", 3499.90m, "https://images.unsplash.com/photo-1511886929837-354d827aae26?w=600&q=80"),
            new(2, "Şampiyonlar Ligi Maç Topu", 1299.50m, "https://images.unsplash.com/photo-1614632537190-23e4146777db?w=600&q=80"),
            new(3, "Kaleci Eldiveni (Grip Pro)", 1899.00m, "https://images.unsplash.com/photo-1579932494958-8683526fbd14?w=600&q=80"),
            new(4, "Taraftar Forması 24/25", 1499.99m, "https://images.unsplash.com/photo-1580087433295-ab2600c1030e?w=600&q=80"),
            new(5, "Elite Futbol Tozluk", 249.90m, "https://images.unsplash.com/photo-1623345805780-8f01f714e65f?w=600&q=80")
        };

        return View(products);
    }
}

/// <summary>
/// Ürün gösterim modeli (Record kullanılarak kısa bir tanımlama yapıldı)
/// </summary>
public record ProductViewModel(int Id, string Name, decimal Price, string ImageUrl);