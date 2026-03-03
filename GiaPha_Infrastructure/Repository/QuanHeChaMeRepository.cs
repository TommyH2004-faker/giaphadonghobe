using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class QuanHeChaMeRepository : IQuanHeChaMeRepository
{
    private readonly DbGiaPha _context;

    public QuanHeChaMeRepository(DbGiaPha context)
    {
        _context = context;
    }

    public async Task<Result<QuanHeChaCon>> CreateAsync(QuanHeChaCon quanHe)
    {
        await _context.QuanHeChaCons.AddAsync(quanHe);
        return Result<QuanHeChaCon>.Success(quanHe);
    }

    public async Task<bool> ExistsAsync(Guid chaMeId, Guid conId, int loaiQuanHe)
    {
        return await _context.QuanHeChaCons
            .AnyAsync(q => q.ChaMeId == chaMeId && q.ConId == conId && q.LoaiQuanHe == loaiQuanHe);
    }

    public async Task<int> CountParentAsync(Guid conId, int loaiQuanHe)
    {
        return await _context.QuanHeChaCons
            .CountAsync(q => q.ConId == conId && q.LoaiQuanHe == loaiQuanHe);
    }

    public async Task<bool> IsLoopAsync(Guid chaMeId, Guid conId)
    {
        return await _context.QuanHeChaCons
            .AnyAsync(q => q.ChaMeId == conId && q.ConId == chaMeId);
    }


}