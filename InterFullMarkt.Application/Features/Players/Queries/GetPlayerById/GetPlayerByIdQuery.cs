namespace InterFullMarkt.Application.Features.Players.Queries.GetPlayerById;

using MediatR;

/// <summary>
/// Get a single player by ID with full details
/// </summary>
public sealed record GetPlayerByIdQuery(Guid PlayerId) : IRequest<GetPlayerByIdResult>;
