using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.GiaPha.Queries.GetMyGiaPhaTree;

public class GetMyGiaPhaTreeQuery : IRequest<Result<GiaPhaTreeResponse>>
{
    public Guid UserId { get; set; }
    public Guid HoId { get; set; } // Bắt buộc vì user có thể thuộc nhiều Ho
    public int MaxLevel { get; set; } = 10;
    public bool IncludeNuGioi { get; set; } = true;
    public bool IncludeDeleted { get; set; } = true; // Mặc định true để hiện người đã xóa, false khi in PDF
}
