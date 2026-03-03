using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.SuKien.Queries.GetAllSuKien;

public class GetAllSuKienHandler 
    : IRequestHandler<GetAllSuKienQuery, Result<List<SuKienResponse>>>
{
    private readonly ISukienRepository _suKienRepository;

    public GetAllSuKienHandler(ISukienRepository suKienRepository)
    {
        _suKienRepository = suKienRepository;
    }

    public async Task<Result<List<SuKienResponse>>> Handle(
        GetAllSuKienQuery request,
        CancellationToken cancellationToken)
    {
        var suKienList = await _suKienRepository.GetAllAsync();

        if (suKienList == null)
        {
            return Result<List<SuKienResponse>>.Failure(
                ErrorType.NotFound,
                "Không có sự kiện nào tồn tại"
            );
        }

        var result = suKienList.Select(s => new SuKienResponse
        {
            Id = s.Id,
            LoaiSuKien = s.LoaiSuKien,
            MoTa = s.MoTa,
            NgayXayRa = s.NgayXayRa,
            DiaDiem = s.DiaDiem,
            ThanhVienId = s.ThanhVienId
        }).ToList();

        return Result<List<SuKienResponse>>.Success(result);
    }
}