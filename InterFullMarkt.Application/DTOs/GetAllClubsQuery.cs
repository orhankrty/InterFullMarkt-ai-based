namespace InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;

using MediatR;
using InterFullMarkt.Application.DTOs;
using System.Collections.Generic;

public record GetAllClubsQuery : IRequest<List<ClubDto>>;