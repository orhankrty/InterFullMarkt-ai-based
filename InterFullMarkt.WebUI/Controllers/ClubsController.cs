namespace InterFullMarkt.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;

public class ClubsController : Controller
{
    private readonly IMediator _mediator;

    public ClubsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var clubs = await _mediator.Send(new GetAllClubsQuery());
        return View(clubs);
    }
}