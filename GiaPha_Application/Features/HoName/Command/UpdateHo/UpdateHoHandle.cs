using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;

namespace GiaPha_Application.Features.HoName.Command.UpdateHo;

public class UpdateHoHandle : IRequestHandler<UpdateHoCommand, Result<HoResponse>>
{
    private readonly IHoRepository _hoRepository;
    public UpdateHoHandle(IHoRepository hoRepository)
    {
        _hoRepository = hoRepository;
    }
    public async Task<Result<HoResponse>> Handle(UpdateHoCommand request, CancellationToken cancellationToken)
    {
        // Lấy họ theo Id
        var ho = await _hoRepository.GetHoByIdAsync(request.Id);
        if (ho == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Họ không tồn tại");
        }

        // Kiểm tra trùng tên với họ khác
        var hoTrungTen = await _hoRepository.GetHoByNameAsync(request.TenHo);

        if (hoTrungTen != null && hoTrungTen.Data != null && ho.Data != null && hoTrungTen.Data.Id != ho.Data.Id)
        {
            return Result<HoResponse>.Failure(ErrorType.Conflict, "Tên họ đã tồn tại");
        }

        // Kiểm tra ThuyToId đã là thủy tổ của họ khác chưa
        if (request.ThuyToId.HasValue && request.ThuyToId.Value != Guid.Empty)
        {
            var hoCoThuyTo = await _hoRepository.GetHoByIdAsync(request.ThuyToId.Value);
            if (hoCoThuyTo != null && hoCoThuyTo.Data != null && ho.Data != null && hoCoThuyTo.Data.Id != ho.Data.Id)
            {
                return Result<HoResponse>.Failure(
                    ErrorType.Conflict, 
                    $"Thành viên này đã là thủy tổ của họ '{hoCoThuyTo.Data.TenHo}'. Một người chỉ có thể là thủy tổ của một họ duy nhất."
                );
            }
        }

        // Cập nhật thông tin
        if (ho.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Dữ liệu Họ không tồn tại");
        }
        // Update(Guid idThuyTo, string tenHo, string moTa , string queQuan )
        ho.Data.Update(request.ThuyToId, request.TenHo, request.MoTa ?? string.Empty, request.queQuan ?? string.Empty);
        var updatedHo = await _hoRepository.UpdateHoAsync(ho.Data);
        if (updatedHo == null || updatedHo.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.Failure, "Cập nhật Họ thất bại");
        }
        return Result<HoResponse>.Success(new HoResponse { TenHo = updatedHo.Data.TenHo, MoTa = updatedHo.Data.MoTa });
    }
}