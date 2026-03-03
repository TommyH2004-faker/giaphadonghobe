using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Command.Delete;

public record DeleteThanhVienCommand(Guid Id) : IRequest<Result<bool>>;
