using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.QuanheChaMe.Command.Create;

public class CreateQuanHeChaConCommand : IRequest<Result<QuanHeChaConResponse>>
{
    public Guid ChaMeId { get; set; }
    public Guid ConId { get; set; }
    public int LoaiQuanHe { get; set; } // 0: Cha, 1: Mẹ
}