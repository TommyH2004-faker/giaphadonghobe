using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Features.QuanheChaMe.Command.Create;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.QuanheChaMe.Command.Create;

public class CreateQuanHeChaConHandle : IRequestHandler<CreateQuanHeChaConCommand, Result<QuanHeChaConResponse>>
{
    private readonly IQuanHeChaMeRepository _quanHeChaConRepository;
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateQuanHeChaConHandle(
        IQuanHeChaMeRepository quanHeChaConRepository,
        IThanhVienRepository thanhVienRepository,
        IUnitOfWork unitOfWork)
    {
        _quanHeChaConRepository = quanHeChaConRepository;
        _thanhVienRepository = thanhVienRepository;
        _unitOfWork = unitOfWork;
    }
public async Task<Result<QuanHeChaConResponse>> Handle(CreateQuanHeChaConCommand request, CancellationToken cancellationToken)
{
    // 1. Không chính mình
    if (request.ChaMeId == request.ConId)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Không thể tạo quan hệ với chính mình.");
    var loaiQuanHe = request.LoaiQuanHe;
    // 2. Không trùng
    var existed = await _quanHeChaConRepository.ExistsAsync(request.ChaMeId, request.ConId, request.LoaiQuanHe);
    if (existed && loaiQuanHe ==0)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Quan hệ cha con đã tồn tại.");
    if (existed && loaiQuanHe ==1)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Quan hệ mẹ con đã tồn tại.");

    // 3. Không vòng lặp
    var isLoop = await _quanHeChaConRepository.IsLoopAsync(request.ChaMeId, request.ConId);
    if (isLoop)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Không thể tạo quan hệ vòng lặp.");

    // 4. Tối đa 1 cha, 1 mẹ
    var count = await _quanHeChaConRepository.CountParentAsync(request.ConId, request.LoaiQuanHe);
    if (count >= 1)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, $"Một người chỉ có tối đa 1 {(request.LoaiQuanHe == 0 ? "cha" : "mẹ")}.");

    // 5. Kiểm tra tồn tại và cùng họ (chỉ áp dụng cho cha-con)
    var chaMe = await _thanhVienRepository.GetHoById(request.ChaMeId);
    var con = await _thanhVienRepository.GetHoById(request.ConId);
    
    if (chaMe == null || chaMe.Data == null)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Cha/Mẹ không tồn tại.");
    
    if (con == null || con.Data == null)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Con không tồn tại.");
    
    // Chỉ kiểm tra cùng họ nếu là quan hệ cha-con (LoaiQuanHe = 0)
    if (request.LoaiQuanHe == 0 && chaMe.Data.HoId != con.Data.HoId)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Cha và con phải cùng họ.");

    // Tạo quan hệ
    var quanHe = GiaPha_Domain.Entities.QuanHeChaCon.Create(request.ChaMeId, request.ConId, request.LoaiQuanHe);
    var created = await _quanHeChaConRepository.CreateAsync(quanHe);
    await _unitOfWork.SaveChangesAsync();
    if (created.Data == null)
        return Result<QuanHeChaConResponse>.Failure(ErrorType.Validation, "Tạo quan hệ cha con thất bại.");

    var response = new QuanHeChaConResponse
    {
        Id = created.Data.Id,
        ChaMeId = created.Data.ChaMeId,
        ConId = created.Data.ConId,
        LoaiQuanHe = created.Data.LoaiQuanHe
    };

    return Result<QuanHeChaConResponse>.Success(response);
}

}