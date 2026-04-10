namespace InterFullMarkt.Application.Features.Dashboard.Queries;

using MediatR;

/// <summary>
/// Get dashboard widgets data (top players, value gainers, league stats)
/// </summary>
public sealed record GetDashboardQuery : IRequest<GetDashboardResult>;
