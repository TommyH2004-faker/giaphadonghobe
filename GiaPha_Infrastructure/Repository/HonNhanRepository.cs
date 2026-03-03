using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class HonNhanRepository : IHonNhanRepository
{
    private readonly DbGiaPha _context;

    public HonNhanRepository(DbGiaPha context)
    {
        _context = context;
    }

    public async Task<Result<HonNhan>> CreateAsync(HonNhan honNhan)
    {
        await _context.HonNhans.AddAsync(honNhan);
        return Result<HonNhan>.Success(honNhan);
    }

    public async Task<bool> ExistsAsync(Guid chongId, Guid voId)
    {
        return await _context.HonNhans
            .AnyAsync(h => (h.ChongId == chongId && h.VoId == voId) || 
                          (h.ChongId == voId && h.VoId == chongId));
    }

    public async Task<bool> HasActiveMarriageAsync(Guid thanhVienId)
    {
        return await _context.HonNhans
            .AnyAsync(h => (h.ChongId == thanhVienId || h.VoId == thanhVienId) && h.TrangThai);
    }

    public async Task<bool> IsBloodRelatedAsync(Guid nguoi1Id, Guid nguoi2Id)
    {
        // Kiểm tra quan hệ huyết thống qua bảng QuanHeChaCon
        var isRelated = await _context.QuanHeChaCons
            .AnyAsync(q => (q.ChaMeId == nguoi1Id && q.ConId == nguoi2Id) ||
                          (q.ChaMeId == nguoi2Id && q.ConId == nguoi1Id));
        
        return isRelated;
    }

    public async Task<Result<HonNhan>> GetByIdAsync(Guid id)
    {
        var honNhan = await _context.HonNhans.FindAsync(id);
        return honNhan != null 
            ? Result<HonNhan>.Success(honNhan) 
            : Result<HonNhan>.Failure(ErrorType.NotFound, "Hôn nhân không tồn tại");
    }

    public async Task<Result<HonNhan>> GetActiveMarriageAsync(Guid chongId, Guid voId)
    {
        var honNhan = await _context.HonNhans
            .FirstOrDefaultAsync(h => h.ChongId == chongId && h.VoId == voId && h.TrangThai);
        
        return honNhan != null 
            ? Result<HonNhan>.Success(honNhan) 
            : Result<HonNhan>.Failure(ErrorType.NotFound, "Hôn nhân không tồn tại hoặc đã kết thúc");
    }

    public Task<Result<HonNhan>> GetActiveHonNhanByMemberId(Guid id)
    {
        var honNhan = _context.HonNhans
            .FirstOrDefault(h => (h.ChongId == id || h.VoId == id) && h.TrangThai);
        
        return honNhan != null 
            ? Task.FromResult(Result<HonNhan>.Success(honNhan)) 
            : Task.FromResult(Result<HonNhan>.Failure(ErrorType.NotFound, "Không tìm thấy hôn nhân đang hoạt động cho thành viên này"));
    }
}