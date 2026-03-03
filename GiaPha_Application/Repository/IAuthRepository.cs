using GiaPha_Application.Common;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository
{
    public interface IAuthRepository
    {
         Task<TaiKhoanNguoiDung>CreateUserAsync(TaiKhoanNguoiDung user);
        Task<TaiKhoanNguoiDung?> GetUserByIdAsync(Guid id);
        Task<TaiKhoanNguoiDung?> GetUserByUsernameAsync(string TenDangNhap);
        Task<TaiKhoanNguoiDung?> GetUserByEmailAsync(string email);
        
        Task<IEnumerable<TaiKhoanNguoiDung>> GetAllUsersAsync();
        Task<TaiKhoanNguoiDung> UpdateUserAsync(TaiKhoanNguoiDung user);
        Task<bool> DeleteUserAsync(int id);
        
        Task <TaiKhoanNguoiDung> AddUserAsync(TaiKhoanNguoiDung newUser);
        
        // Thêm method để link User với Ho
        Task<TaiKhoan_Ho> AddUserToHoAsync(Guid taiKhoanId, Guid hoId, RoleCuaHo roleInHo);
        
        // Lấy email Trưởng họ để gửi notification
        Task<string?> GetTruongHoEmailByHoIdAsync(Guid hoId);
    }
}