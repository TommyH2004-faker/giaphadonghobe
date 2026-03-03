using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.YeuCau.Commands.XinVaoHo;

public record XinVaoHoCommand(Guid UserId, Guid HoId, string LyDoXinVao) : IRequest<Result<Guid>>;
