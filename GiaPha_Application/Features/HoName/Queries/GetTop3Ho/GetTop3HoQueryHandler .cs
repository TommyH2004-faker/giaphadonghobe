using MediatR;

using GiaPha_Application.DTOs;
using GiaPha_Application.Common;
using GiaPha_Application.Repository;

public class GetTop3HoQueryHandler 
    : IRequestHandler<GetTop3HoQuery, Result<List<HoResponse>>>
{
    private readonly  IHoRepository hoRepository;

    public GetTop3HoQueryHandler(IHoRepository hoRepository)
    {
        this.hoRepository = hoRepository;
    }

    public async Task<Result<List<HoResponse>>> Handle(GetTop3HoQuery request, CancellationToken cancellationToken)
    {
        var hosResult = await hoRepository.GetTop3HoAsync();
        if (hosResult == null || hosResult.Data == null || !hosResult.Data.Any())
        {
            return Result<List<HoResponse>>.Failure(ErrorType.NotFound, "Không có họ nào tồn tại");
        }

        var hoResponses = hosResult.Data.Select(h => new HoResponse
        {
            Id = h.Id,
            TenHo = h.TenHo,
            MoTa = h.MoTa,
            HinhAnh = h.HinhAnh
        }).ToList();

        return Result<List<HoResponse>>.Success(hoResponses);
    }
}
