using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;
using InterFullMarkt.Application.Features.Players.Queries.GetAllPlayers;
using InterFullMarkt.Application.Features.Clubs.Commands.CreateClub;
using InterFullMarkt.Application.Features.Clubs.Commands.UpdateClub;
using InterFullMarkt.Application.Features.Clubs.Commands.DeleteClub;
using InterFullMarkt.Application.Features.Clubs.Queries.GetClubById;
using InterFullMarkt.Application.Features.Players.Commands.UpdatePlayer;
using InterFullMarkt.Application.Features.Players.Commands.DeletePlayer;
using InterFullMarkt.Application.Features.Players.Queries.GetPlayerById;
using InterFullMarkt.Application.Features.Leagues.Queries.GetAllLeagues;
using InterFullMarkt.Application.DTOs;

namespace InterFullMarkt.WebUI.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IMediator mediator, ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var clubs = await _mediator.Send(new GetAllClubsQuery());
        var playersResult = await _mediator.Send(new GetAllPlayersQuery { PageSize = 1000 });
        
        ViewBag.ClubCount = clubs.Count;
        ViewBag.PlayerCount = playersResult.TotalCount;
        
        return View();
    }

    [Route("Teams")]
    public async Task<IActionResult> Teams()
    {
        var clubs = await _mediator.Send(new GetAllClubsQuery());
        return View(clubs);
    }

    [HttpGet("Teams/Create")]
    public async Task<IActionResult> CreateTeam()
    {
        ViewBag.Leagues = await _mediator.Send(new GetAllLeaguesQuery());
        return View(new CreateClubDto());
    }

    [HttpPost("Teams/Create")]
    public async Task<IActionResult> CreateTeam(CreateClubDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Leagues = await _mediator.Send(new GetAllLeaguesQuery());
            return View(dto);
        }

        await _mediator.Send(new CreateClubCommand(dto, User.Identity?.Name ?? "Admin"));
        TempData["Success"] = "Takım başarıyla eklendi.";
        return RedirectToAction(nameof(Teams));
    }

    [HttpGet("Teams/Edit/{id}")]
    public async Task<IActionResult> EditTeam(Guid id)
    {
        var club = await _mediator.Send(new GetClubByIdQuery(id));
        if (club == null) return NotFound();

        ViewBag.Leagues = await _mediator.Send(new GetAllLeaguesQuery());
        
        var updateDto = new UpdateClubDto
        {
            Id = club.Id,
            Name = club.Name,
            ShortName = club.ShortName,
            FoundingYear = club.FoundingYear,
            StadiumName = club.StadiumName,
            Colors = club.Colors,
            LogoUrl = club.LogoUrl,
            BudgetAmount = club.BudgetAmount ?? 0,
            Currency = "EUR",
            LeagueId = Guid.Empty // Logic to find league if needed, but for simplicity...
        };

        return View(updateDto);
    }

    [HttpPost("Teams/Edit/{id}")]
    public async Task<IActionResult> EditTeam(UpdateClubDto dto)
    {
        await _mediator.Send(new UpdateClubCommand(dto, User.Identity?.Name ?? "Admin"));
        TempData["Success"] = "Takım başarıyla güncellendi.";
        return RedirectToAction(nameof(Teams));
    }

    [HttpPost("Teams/Delete/{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        await _mediator.Send(new DeleteClubCommand(id, User.Identity?.Name ?? "Admin"));
        return Json(new { success = true });
    }

    [Route("Players")]
    public async Task<IActionResult> Players()
    {
        var result = await _mediator.Send(new GetAllPlayersQuery { PageSize = 1000 });
        return View(result.Players);
    }

    [HttpGet("Players/Edit/{id}")]
    public async Task<IActionResult> EditPlayer(Guid id)
    {
        var result = await _mediator.Send(new GetPlayerByIdQuery(id));
        if (result == null) return NotFound();

        var updateDto = new UpdatePlayerDto
        {
            Id = result.PlayerId,
            FullName = result.FullName,
            Position = 1, // Logic to convert string back to enum value if needed
            NationalityCode = "TR", // Mock or extract from string
            DateOfBirth = DateTime.Today.AddYears(-result.Age),
            Height = result.Height,
            Weight = (int)result.Weight,
            ImageUrl = result.ImageUrl,
            MarketValueAmount = result.MarketValueAmount,
            Currency = result.MarketValueCurrency
        };

        return View(updateDto);
    }

    [HttpPost("Players/Edit/{id}")]
    public async Task<IActionResult> EditPlayer(UpdatePlayerDto dto)
    {
        await _mediator.Send(new UpdatePlayerCommand(dto, User.Identity?.Name ?? "Admin"));
        TempData["Success"] = "Oyuncu başarıyla güncellendi.";
        return RedirectToAction(nameof(Players));
    }

    [HttpPost("Players/Delete/{id}")]
    public async Task<IActionResult> DeletePlayer(Guid id)
    {
        await _mediator.Send(new DeletePlayerCommand(id, User.Identity?.Name ?? "Admin"));
        return Json(new { success = true });
    }
}
