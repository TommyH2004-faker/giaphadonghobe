using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.SuKien.Command.Create;
// ...existing code...
public class CreateSuKienHandler : IRequestHandler<CreateSuKienCommand, Result<SuKienResponse>>
{
    private readonly ISukienRepository _suKienRepository;

    public CreateSuKienHandler(ISukienRepository suKienRepository)
    {
        _suKienRepository = suKienRepository;
    }

    public async Task<Result<SuKienResponse>> Handle(CreateSuKienCommand request, CancellationToken cancellationToken)
    {
        var suKien = GiaPha_Domain.Entities.SuKien.Create(
                request.ThanhVienId,
                request.LoaiSuKien,
                request.NgayXayRa,
                request.DiaDiem,
                request.MoTa
            );
        var result = await _suKienRepository.CreateEventAsync(suKien);
        if (result != null)
        {
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
        else
        {
            return Result<SuKienResponse>.Failure(ErrorType.InternalServerError, "Lỗi khi tạo sự kiện");
        }
    }
}