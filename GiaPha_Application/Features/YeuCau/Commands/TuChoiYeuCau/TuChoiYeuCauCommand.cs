using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.YeuCau.Commands.TuChoiYeuCau;

public record TuChoiYeuCauCommand(Guid YeuCauId, Guid NguoiXuLyId, string? GhiChu) : IRequest<Result<bool>>;
