using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.TaiKhoanNguoiDungs.Queries.GetTaiKhoanNguoiDungByid;
public class GetTaiKhoanNguoiDungHandle : IRequestHandler<GetTaiKhoanNguoiDungQueryById, Result<TaiKhoanNguoiDungResponse>>
{
    private readonly IAuthRepository authRepository;

    public GetTaiKhoanNguoiDungHandle(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }
    
   public async Task<Result<TaiKhoanNguoiDungResponse>> Handle(
    GetTaiKhoanNguoiDungQueryById request,
    CancellationToken cancellationToken)
    {
        var entity = await authRepository.GetUserByIdAsync(request.Id);

        if (entity == null)
        {
            return Result<TaiKhoanNguoiDungResponse>
                .Failure(ErrorType.NotFound, "USER_NOT_FOUND");
        }

        // MAP ENTITY -> DTO
        var response = new TaiKhoanNguoiDungResponse
        {
            Id = entity.Id,
            TenDangNhap = entity.TenDangNhap,
            Gmail = entity.Email,
            Enabled = entity.Enabled,
            Avatar = entity.Avatar,
            GioiTinh = entity.GioiTinh,
            SoDienThoai = entity.SoDienThoai,
            AvailableHos = entity.TaiKhoan_Hos.Select(th => new HoWithRoleResponse
            {
                Id = th.Ho.Id,
                TenHo = th.Ho.TenHo,
                MoTa = th.Ho.MoTa,
                HinhAnh = th.Ho.HinhAnh,
                QueQuan = th.Ho.QueQuan,
                ThuyToId = th.Ho.ThuyToId,
                RoleInHo = (int)th.RoleInHo
            }).ToList()
        };

        return Result<TaiKhoanNguoiDungResponse>.Success(response);
    }
}