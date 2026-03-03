using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetHoByTruongHoEmail;

public record GetHoByTruongHoEmailQuery(string Email) : IRequest<Result<HoByTruongHoResponse>>;

public record HoByTruongHoResponse
{
    public string HoId { get; init; } = null!;
    public string TenHo { get; init; } = null!;
    public string TruongHoEmail { get; init; } = null!;
    public string TruongHoName { get; init; } = null!;
}
