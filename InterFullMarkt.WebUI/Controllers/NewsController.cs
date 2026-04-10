using Microsoft.AspNetCore.Mvc;

namespace InterFullMarkt.WebUI.Controllers;

public class NewsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
