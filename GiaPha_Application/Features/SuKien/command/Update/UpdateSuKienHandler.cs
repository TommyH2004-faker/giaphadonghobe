using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.SuKien.Command.Update;

public class UpdateSuKienHandler : IRequestHandler<UpdateSuKienCommand, Result<SuKienResponse>>
{
    private readonly ISukienRepository _suKienRepository;

    public UpdateSuKienHandler(ISukienRepository suKienRepository)
    {
        _suKienRepository = suKienRepository;
    }

    public async Task<Result<SuKienResponse>> Handle(UpdateSuKienCommand request, CancellationToken cancellationToken)
    {
        var existing = await _suKienRepository.GetEventByIdAsync(request.Id);
        if (existing == null)
        {
            return Result<SuKienResponse>.Failure(ErrorType.NotFound, "Không tìm thấy sự kiện.");
        }

        existing.Update(request.ThanhVienId, request.LoaiSuKien, request.NgayXayRa, request.DiaDiem, request.MoTa);
        var result = await _suKienRepository.UpdateEventAsync(existing);

        var response = new SuKienResponse
        {
            Id = result.Id,
            ThanhVienId = result.ThanhVienId,
            LoaiSuKien = result.LoaiSuKien,
            NgayXayRa = result.NgayXayRa,
            DiaDiem = result.DiaDiem,
            MoTa = result.MoTa
        };
        return Result<SuKienResponse>.Success(response);
    }
}
