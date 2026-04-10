namespace InterFullMarkt.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;
using InterFullMarkt.Application.Features.Players.Queries.GetAllPlayers;
using InterFullMarkt.Application.DTOs;

/// <summary>
/// Oyuncu yönetimi controller'ı.
/// MediatR CQRS pattern'ını kullanarak Command ve Query'leri işler.
/// </summary>
public sealed class PlayersController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(IMediator mediator, ILogger<PlayersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Tüm oyuncuları listeler.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string? search = null, string? sortBy = null)
    {
        try
        {
            _logger.LogInformation("Oyuncular sayfası istendi: Page={Page}, Search={Search}", page, search);

            var query = new GetAllPlayersQuery
            {
                PageIndex = page - 1,
                PageSize = 12,
                SearchTerm = search,
                SortBy = sortBy ?? "name"
            };

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage ?? "Oyuncuları yükleme sırasında bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["CurrentPage"] = page;
            ViewData["SearchTerm"] = search;
            ViewData["SortBy"] = sortBy ?? "name";

            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oyuncuları listeleyemedi");
            TempData["Error"] = "Oyuncuları yükleme sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// Yeni oyuncu oluşturma formunu gösterir.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePlayerDto());
    }

    /// <summary>
    /// Yeni oyuncu oluşturur.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePlayerDto createPlayerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(createPlayerDto);
            }

            _logger.LogInformation("Yeni oyuncu oluşturma isteği: {FullName}", createPlayerDto.FullName);

            var command = new CreatePlayerCommand(createPlayerDto, User?.Identity?.Name ?? "System");
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Oyuncu oluşturulamadı.");
                return View(createPlayerDto);
            }

            TempData["Success"] = $"{createPlayerDto.FullName} başarıyla sisteme eklendi!";
            _logger.LogInformation("Oyuncu başarıyla oluşturuldu: {PlayerId}", result.PlayerId);

            return RedirectToAction("Index");
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validasyon hatası: {Message}", ex.Message);
            
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(createPlayerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oyuncu oluşturulurken hata: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, "Oyuncu oluşturulurken bir hata oluştu.");
            return View(createPlayerDto);
        }
    }

    /// <summary>
    /// Oyuncu detaylarını gösterir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            // TODO: GetPlayerByIdQuery oluştur
            return Ok("Oyuncu detayları henüz implementasyonda değildir.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oyuncu detaylarını getirirken hata");
            return RedirectToAction("Index");
        }
    }
}
