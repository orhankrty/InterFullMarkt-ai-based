namespace InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;

using MediatR;
using InterFullMarkt.Application.DTOs;

public sealed class GetAllClubsQuery : IRequest<List<ClubDto>>
{
}
