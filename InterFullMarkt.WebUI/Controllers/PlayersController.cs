namespace InterFullMarkt.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;
using InterFullMarkt.Application.DTOs;

/// <summary>
/// Oyuncu yönetimi API endpoint'leri.
/// MediatR Command Handler'ları üzerinden işlem yapılır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(IMediator mediator, ILogger<PlayersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Yeni oyuncu oluşturur.
    /// </summary>
    /// <param name="createPlayerDto">Oyuncu oluşturma verisi</param>
    /// <returns>Oluşturulan oyuncu ID'si</returns>
    /// <response code="200">Oyuncu başarıyla oluşturuldu</response>
    /// <response code="400">Validasyon hatası</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatePlayerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(type: typeof(ProblemDetails), statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto createPlayerDto, CancellationToken cancellationToken = default)
    {
        if (createPlayerDto is null)
            return BadRequest(new { message = "Oyuncu verisi boş olamaz" });

        try
        {
            _logger.LogInformation(
                "Yeni oyuncu oluşturma isteği: {FullName}, Pozisyon: {Position}",
                createPlayerDto.FullName,
                createPlayerDto.Position);

            var command = new CreatePlayerCommand(createPlayerDto, "Api.Client");
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage, code = result.ErrorCode });

            _logger.LogInformation("Oyuncu başarıyla oluşturuldu: {PlayerId}", result.PlayerId);

            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validasyon hatası: {Errors}", string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new
            {
                message = "Validasyon hatası",
                errors = ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oyuncu oluşturulurken hata: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Sunucuda bir hata oluştu",
                detail = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? ex.Message : null
            });
        }
    }
}
