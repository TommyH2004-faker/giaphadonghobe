using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Queries.GetByHoid;
public class GetThanhVienByHoIdHandle : IRequestHandler<GetThanhVienByHoIdQuery, List<ThanhVienResponse>>
{
    private readonly IThanhVienRepository _thanhVienRepository;

    public GetThanhVienByHoIdHandle(IThanhVienRepository thanhVienRepository)
    {
        _thanhVienRepository = thanhVienRepository;
    }

    public async Task<List<ThanhVienResponse>> Handle(GetThanhVienByHoIdQuery request, CancellationToken cancellationToken)
    {
        var thanhViens = await _thanhVienRepository.GetThanhVienByHoId(request.HoId);

        var responses = thanhViens.Select(member => new ThanhVienResponse
        {
            Id = member.Id,
            Avatar = member.Avatar,
            HoTen = member.HoTen,
            GioiTinh = member.GioiTinh,
            NgaySinh = member.NgaySinh,
            NoiSinh = member.NoiSinh,
            NgayMat = member.NgayMat,
            NoiMat = member.NoiMat,
            DoiThu = 0,
            TieuSu = member.TieuSu,
            TrangThai = member.TrangThai,
            HoId = member.HoId ?? Guid.Empty
        }).ToList();
        return responses;
    }
}