using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly DbGiaPha _context;
    public AuthRepository(DbGiaPha context)
    {
        _context = context;
    }

    public async Task<TaiKhoanNguoiDung> AddUserAsync(TaiKhoanNguoiDung newUser)
    {
        _context.TaiKhoanNguoiDungs.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<TaiKhoanNguoiDung> CreateUserAsync(TaiKhoanNguoiDung user)
    {
        _context.TaiKhoanNguoiDungs.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.TaiKhoanNguoiDungs.FindAsync(id);
        if (user == null)
        {
            return false;
        }
        _context.TaiKhoanNguoiDungs.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TaiKhoanNguoiDung>> GetAllUsersAsync()
    {
        var users = await _context.TaiKhoanNguoiDungs.ToListAsync();
        return users;
    }

    public async Task<TaiKhoanNguoiDung?> GetUserByEmailAsync(string email)
    {
        var user = await _context.TaiKhoanNguoiDungs
            .Include(u => u.TaiKhoan_Hos)
                .ThenInclude(th => th.Ho)
            .FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<TaiKhoanNguoiDung?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.TaiKhoanNguoiDungs
            .Include(u => u.TaiKhoan_Hos)
                .ThenInclude(th => th.Ho)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task<TaiKhoanNguoiDung?> GetUserByUsernameAsync(string TenDangNhap)
    {
        var user = await _context.TaiKhoanNguoiDungs
            .Include(u => u.TaiKhoan_Hos)
                .ThenInclude(th => th.Ho)
            .FirstOrDefaultAsync(u => u.TenDangNhap == TenDangNhap);
        return user;
    }

   

    public async Task<TaiKhoanNguoiDung> UpdateUserAsync(TaiKhoanNguoiDung user)
    {
        _context.TaiKhoanNguoiDungs.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<TaiKhoan_Ho> AddUserToHoAsync(Guid taiKhoanId, Guid hoId, RoleCuaHo roleInHo)
    {
        var taiKhoanHo = new TaiKhoan_Ho
        {
            TaiKhoanId = taiKhoanId,
            HoId = hoId,
            RoleInHo = roleInHo,
            NgayThamGia = DateTime.UtcNow
        };
        _context.TaiKhoan_Hos.Add(taiKhoanHo);
        await _context.SaveChangesAsync();  
        return taiKhoanHo;
    }

    public async Task<string?> GetTruongHoEmailByHoIdAsync(Guid hoId)
    {
        var truongHo = await _context.TaiKhoan_Hos
            .Include(th => th.TaiKhoan)
            .Where(th => th.HoId == hoId && th.RoleInHo == RoleCuaHo.TruongHo)
            .Select(th => th.TaiKhoan.Email)
            .FirstOrDefaultAsync();
            
        return truongHo;
    }

}