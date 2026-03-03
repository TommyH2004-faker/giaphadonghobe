using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.YeuCau.Commands.DuyetYeuCau;

public record DuyetYeuCauCommand(Guid YeuCauId, Guid NguoiXuLyId) : IRequest<Result<bool>>;
