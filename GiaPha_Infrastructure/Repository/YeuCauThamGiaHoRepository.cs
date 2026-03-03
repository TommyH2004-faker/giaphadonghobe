using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class YeuCauThamGiaHoRepository : IYeuCauThamGiaHoRepository
{
    private readonly DbGiaPha _context;
    public YeuCauThamGiaHoRepository(DbGiaPha context) => _context = context;

    public async Task AddAsync(YeuCauThamGiaHo yeuCau)
    {
        await _context.YeuCauThamGiaHos.AddAsync(yeuCau);
    }

    public async Task<YeuCauThamGiaHo?> GetByIdAsync(Guid id)
    {
        return await _context.YeuCauThamGiaHos
            .Include(y => y.User)
            .Include(y => y.Ho)
            .FirstOrDefaultAsync(y => y.Id == id);
    }

    public async Task<IReadOnlyList<YeuCauThamGiaHo>> GetPendingByHoIdAsync(Guid hoId)
    {
        return await _context.YeuCauThamGiaHos
            .Include(y => y.User)
            .Where(y => y.HoId == hoId && y.TrangThai == TrangThaiYeuCau.DangCho)
            .OrderByDescending(y => y.NgayTao)
            .ToListAsync();
    }

    public async Task<bool> ExistsPendingAsync(Guid userId, Guid hoId)
    {
        return await _context.YeuCauThamGiaHos
            .AnyAsync(y => y.UserId == userId && y.HoId == hoId && y.TrangThai == TrangThaiYeuCau.DangCho);
    }
}
