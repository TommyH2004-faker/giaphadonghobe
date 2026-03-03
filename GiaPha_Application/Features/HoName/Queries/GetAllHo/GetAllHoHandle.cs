
using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetAllHo;
public class GetAllHoHandle : IRequestHandler<GetAllHoQuery, Result<List<HoResponse>>>
{
    private readonly IHoRepository _hoRepository;
    public GetAllHoHandle(IHoRepository hoRepository)
    {
        _hoRepository = hoRepository;
    }
    public async Task<Result<List<HoResponse>>> Handle(GetAllHoQuery request, CancellationToken cancellationToken)
    {
        var hos = await _hoRepository.GetAllHoAsync();
        if (hos == null || hos.Data == null || !hos.Data.Any())
        {
            return Result<List<HoResponse>>.Failure(ErrorType.NotFound, "Không có họ nào tồn tại");
        }
        var hoResponses = hos.Data.Select(h => new HoResponse
        {
            Id = h.Id,
            TenHo = h.TenHo,
            MoTa = h.MoTa,
            HinhAnh = h.HinhAnh
        }).ToList();
        return Result<List<HoResponse>>.Success(hoResponses);
    }
}
