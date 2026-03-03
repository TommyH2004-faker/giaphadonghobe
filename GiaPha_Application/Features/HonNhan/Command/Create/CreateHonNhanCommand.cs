using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HonNhan.Command.Create;

public class CreateHonNhanCommand : IRequest<Result<HonNhanResponse>>
{
    public Guid ChongId { get; set; }
    public Guid VoId { get; set; }
    public DateTime NgayKetHon { get; set; }
}