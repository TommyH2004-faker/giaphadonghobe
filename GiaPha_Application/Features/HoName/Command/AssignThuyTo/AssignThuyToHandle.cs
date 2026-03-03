using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.HoName.Command.AssignThuyTo;
public class AssignThuyToHandle : IRequestHandler<AssignThuyToCommand, Result<HoResponse>>
{
    private readonly IHoRepository _hoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignThuyToHandle(IHoRepository hoRepository , IUnitOfWork unitOfWork)
    {
        _hoRepository = hoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HoResponse>> Handle(AssignThuyToCommand request, CancellationToken cancellationToken)
    {
        var ho = await _hoRepository.GetHoByIdAsync(request.HoId);
        if (ho == null || ho.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Dữ liệu Họ không tồn tại");
        }
        // check thủy tổ đã được gán cho họ khác chưa
        var hoWithThuyTo = await _hoRepository.GetHoByIdAsync(request.ThuyToId);
        if (hoWithThuyTo != null && hoWithThuyTo.Data != null && hoWithThuyTo.Data.Id != ho.Data.Id)
        {
            return Result<HoResponse>.Failure(
                ErrorType.Conflict,
                $"Thành viên này đã là thủy tổ của họ '{hoWithThuyTo.Data.TenHo}'. Một người chỉ có thể là thủy tổ của một họ duy nhất."
            );
        }
        ho.Data.AssignThuyTo(request.ThuyToId);
        
        // ⚡ Domain Event sẽ tự động được dispatch trong SaveChangesAsync()
        var updatedHo = await _hoRepository.UpdateHoAsync(ho.Data);
        if (updatedHo == null || updatedHo.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Cập nhật Thủy Tổ cho Họ thất bại");
        } 
        await _unitOfWork.SaveChangesAsync();


        var hoResponse = new HoResponse
        {
            Id = updatedHo.Data.Id,
            TenHo = updatedHo.Data.TenHo,
            MoTa = updatedHo.Data.MoTa,
            QueQuan = updatedHo.Data.QueQuan,
            ThuyToId = updatedHo.Data.ThuyToId
        };
        
        return Result<HoResponse>.Success(hoResponse);
    }
}