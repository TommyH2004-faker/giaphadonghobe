using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetById;

public class GetHoHandle : IRequestHandler<GetHoByIdQuery, Result<HoResponse>>
{
    private readonly IHoRepository _hoRepository;
    public GetHoHandle(IHoRepository hoRepository)
    {
        _hoRepository = hoRepository;
    }
    public async Task<Result<HoResponse>> Handle(GetHoByIdQuery request, CancellationToken cancellationToken)
    {
        var ho = await _hoRepository.GetHoByIdAsync(request.Id);
        if (ho == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Họ không tồn tại");
        }
        if(ho.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Dữ liệu Họ không tồn tại");
        }
        return Result<HoResponse>.Success(new HoResponse { TenHo = ho.Data.TenHo, MoTa = ho.Data.MoTa , HinhAnh= ho.Data.HinhAnh});
    }
}