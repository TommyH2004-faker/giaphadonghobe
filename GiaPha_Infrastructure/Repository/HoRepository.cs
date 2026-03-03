
using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class HoRepository : IHoRepository
{
    private readonly DbGiaPha _context;
    public HoRepository(DbGiaPha context)
    {
        _context = context;
    }

   

    public async Task<Result<IEnumerable<Ho>>> GetAllHoAsync()
    {
        return Result<IEnumerable<Ho>>.Success(await _context.Hos.ToListAsync());
    }
    public async Task<Result<bool>> DeleteHoAsync(Guid id)
    {
        var ho = await _context.Hos.FindAsync(id);
        if (ho == null)
        {
            return Result<bool>.Failure(ErrorType.NotFound, "Hộ không tồn tại");
        }
        _context.Hos.Remove(ho);
        return Result<bool>.Success(true);   
    }
    public async Task<Result<Ho?>> GetHoByIdAsync(Guid hoId)
    {
        var ho = await _context.Hos
            .Include(h => h.ThanhViens)
            .FirstOrDefaultAsync(h => h.Id == hoId);
        return Result<Ho?>.Success(ho);
    }

    public async Task<Result<Ho?>> GetHoByNameAsync(string tenHo)
    {
        return Result<Ho?>.Success(await _context.Hos.FirstOrDefaultAsync(h => h.TenHo == tenHo));
    }

    // public async Task<Result<Ho?>> GetHoByThuyToIdAsync(Guid thuyToId)
    // {
    //     return Result<Ho?>.Success(await _context.Hos.FirstOrDefaultAsync(h => h.ThuyToId == thuyToId));
    // }
    // ...existing code...
    public async Task<Result<Ho?>> GetHoByThuyToIdAsync(Guid thuyToId)
    {
        var ho = await _context.Hos
            .Include(h => h.ThuyTo)
            .FirstOrDefaultAsync(h => h.ThuyToId == thuyToId);
        
        if (ho == null)
            return Result<Ho?>.Failure(ErrorType.NotFound, "Không tìm thấy họ");
        
        return Result<Ho?>.Success(ho);
    }
    public async Task<Result<Ho>> UpdateHoAsync(Ho ho)
    {
        _context.Hos.Update(ho);
        return Result<Ho>.Success(ho);
    }

    public async Task<Result<List<Ho>>> GetTop3HoAsync()
    {
        var top3Hos = await _context.Hos.Take(3).ToListAsync();
        return Result<List<Ho>>.Success(top3Hos);
    }

    public Task<Result<List<Ho>>> GetHosByThanhVienIdAsync(Guid thanhVienId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Ho>>> GetHosByUserIdAsync(Guid userId)
    {
        var hos = await _context.TaiKhoan_Hos
            .Where(th => th.TaiKhoanId == userId)
            .Include(th => th.Ho)
            .Select(th => th.Ho)
            .ToListAsync();
        
        return Result<List<Ho>>.Success(hos);
    }

    public async Task<Result<Ho?>> CreateHoAsync(string tenHo, string? moTa, string? queQuan)
    {
        var ho = Ho.Create(tenHo, moTa, queQuan);
        _context.Hos.Add(ho);
        return Result<Ho?>.Success(ho);
    }

    public async Task<Result<bool>> AddUserToHoAsync(Guid userId, Guid hoId, RoleCuaHo role)
    {
        try
        {
            var taiKhoanHo = new TaiKhoan_Ho
            {
                TaiKhoanId = userId,
                HoId = hoId,
                RoleInHo = role,
                NgayThamGia = DateTime.UtcNow
            };

            await _context.TaiKhoan_Hos.AddAsync(taiKhoanHo);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ErrorType.InternalError, $"Lỗi khi thêm người dùng vào họ: {ex.Message}");
        }
    }

    public async Task<Result<string?>> GetTruongHoEmailAsync(Guid hoId)
    {
        var email = await _context.TaiKhoan_Hos
            .Where(th => th.HoId == hoId && th.RoleInHo == RoleCuaHo.TruongHo)
            .Include(th => th.TaiKhoan)
            .Select(th => th.TaiKhoan.Email)
            .FirstOrDefaultAsync();
        
        return Result<string?>.Success(email);
    }
}