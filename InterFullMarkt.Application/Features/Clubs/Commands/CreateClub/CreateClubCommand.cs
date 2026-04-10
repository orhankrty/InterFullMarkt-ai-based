namespace InterFullMarkt.Application.Features.Clubs.Commands.CreateClub;

using MediatR;
using InterFullMarkt.Application.DTOs;

public sealed record CreateClubCommand(CreateClubDto ClubData, string CreatedByUserId) : IRequest<Guid>;
