using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.HoName.Queries.GetHoByUserEmail;

public class GetHoByUserEmailHandler : IRequestHandler<GetHoByUserEmailQuery, Result<HoResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IHoRepository _hoRepository;
    private readonly ILogger<GetHoByUserEmailHandler> _logger;

    public GetHoByUserEmailHandler(
        IAuthRepository authRepository,
        IHoRepository hoRepository,
        ILogger<GetHoByUserEmailHandler> logger)
    {
        _authRepository = authRepository;
        _hoRepository = hoRepository;
        _logger = logger;
    }

    public async Task<Result<HoResponse>> Handle(GetHoByUserEmailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for Ho by user email: {Email}", request.Email);

        // 1. Tìm user theo email
        var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
        if (userResult== null)
        {
            _logger.LogWarning("User not found with email: {Email}", request.Email);
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Không tìm thấy người dùng với email này");
        }

        var user = userResult;

        // 2. Kiểm tra user có thuộc họ nào không
        var taiKhoanHo = user.TaiKhoan_Hos.FirstOrDefault();
        if (taiKhoanHo == null)
        {
            _logger.LogWarning("User {Email} does not belong to any Ho", request.Email);
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Người dùng này chưa thuộc họ nào");
        }

        // 3. Lấy thông tin họ
        var hoResult = await _hoRepository.GetHoByIdAsync(taiKhoanHo.HoId);
        if (!hoResult.IsSuccess || hoResult.Data == null)
        {
            _logger.LogError("Ho not found with ID: {HoId}", taiKhoanHo.HoId);
            return Result<HoResponse>.Failure(ErrorType.NotFound, "Không tìm thấy họ");
        }

        var ho = hoResult.Data;

        var hoResponse = new HoResponse
        {
            Id = ho.Id,
            TenHo = ho.TenHo,
            MoTa = ho.MoTa,
            HinhAnh = ho.HinhAnh,
            QueQuan = ho.QueQuan,
            ThuyToId = ho.ThuyToId
        };

        _logger.LogInformation("Found Ho: {TenHo} (ID: {HoId}) for user {Email}", ho.TenHo, ho.Id, request.Email);
        return Result<HoResponse>.Success(hoResponse);
    }
}
