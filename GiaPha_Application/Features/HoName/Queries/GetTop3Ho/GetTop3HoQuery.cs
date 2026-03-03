using MediatR;
using GiaPha_Application.Common;
using GiaPha_Application.DTOs;

public record GetTop3HoQuery : IRequest<Result<List<HoResponse>>>
{
}
