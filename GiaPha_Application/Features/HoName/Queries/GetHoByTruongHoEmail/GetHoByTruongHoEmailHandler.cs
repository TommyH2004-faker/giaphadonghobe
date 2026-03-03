using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.HoName.Queries.GetHoByTruongHoEmail;

public class GetHoByTruongHoEmailHandler : IRequestHandler<GetHoByTruongHoEmailQuery, Result<HoByTruongHoResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<GetHoByTruongHoEmailHandler> _logger;

    public GetHoByTruongHoEmailHandler(
        IAuthRepository authRepository,
        ILogger<GetHoByTruongHoEmailHandler> logger)
    {
        _authRepository = authRepository;
        _logger = logger;
    }

    public async Task<Result<HoByTruongHoResponse>> Handle(GetHoByTruongHoEmailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(" Searching for Ho by TruongHo email: {Email}", request.Email);

        // 1. Tìm user theo email
        var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
        if (userResult == null)
        {
            _logger.LogWarning(" User not found with email: {Email}", request.Email);
            return Result<HoByTruongHoResponse>.Failure(ErrorType.NotFound, "Không tìm thấy người dùng với email này");
        }

        var user = userResult;

        // 2. Kiểm tra user có phải là Trưởng họ không
        var truongHoRelation = user.TaiKhoan_Hos.FirstOrDefault(th => th.RoleInHo == RoleCuaHo.TruongHo);
        if (truongHoRelation == null)
        {
            _logger.LogWarning(" User {Email} is not a TruongHo of any Ho", request.Email);
            return Result<HoByTruongHoResponse>.Failure(ErrorType.NotFound, "Email này không phải là Trưởng họ của dòng họ nào");
        }

        // 3. Lấy thông tin họ
        var ho = truongHoRelation.Ho;
        if (ho == null)
        {
            _logger.LogError(" Ho not found for TruongHo {Email}", request.Email);
            return Result<HoByTruongHoResponse>.Failure(ErrorType.NotFound, "Không tìm thấy thông tin họ");
        }

        var response = new HoByTruongHoResponse
        {
            HoId = ho.Id.ToString(),
            TenHo = ho.TenHo,
            TruongHoEmail = user.Email,
            TruongHoName = user.TenDangNhap
        };

        _logger.LogInformation(" Found Ho: {TenHo} (ID: {HoId}) with TruongHo {Name}", ho.TenHo, ho.Id, user.TenDangNhap);
        return Result<HoByTruongHoResponse>.Success(response);
    }
}
