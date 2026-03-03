
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;


public class SuKienRepository : ISukienRepository
{
    private readonly DbGiaPha _context;

    public SuKienRepository(DbGiaPha context)
    {
        _context = context;
    }

    public async Task<SuKien> CreateEventAsync(SuKien suKienDto)
    {
        _context.SuKiens.Add(suKienDto);
        await _context.SaveChangesAsync();
        return suKienDto;
    }

    public async Task<bool> DeleteEventAsync(Guid suKienId)
    {
        var suKien = await _context.SuKiens.FindAsync(suKienId);
        if (suKien == null)
        {
            return false;
        }
        _context.SuKiens.Remove(suKien);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SuKien>> GetAllAsync()
    {
        return await _context.SuKiens.ToListAsync();
    }

    public async Task<SuKien> GetEventByIdAsync(Guid suKienId)
    {
        return await _context.SuKiens.FindAsync(suKienId) ?? throw new KeyNotFoundException("Không tìm thấy sự kiện");
    }

    public async Task<List<SuKien>> GetEventsByThanhVienIdAsync(Guid thanhVienId)
    {
        return await _context.SuKiens.Where(x => x.ThanhVienId == thanhVienId).ToListAsync();
    }

    public async Task<List<SuKien>> GetUpcomingEventsAsync(int daysAhead)
    {
        var now = DateTime.UtcNow;
        var toDate = now.AddDays(daysAhead);
        return await _context.SuKiens.Where(x => x.NgayXayRa >= now && x.NgayXayRa <= toDate).ToListAsync();
    }

    public async Task<SuKien> UpdateEventAsync(SuKien suKienDto)
    {
        var suKien = await _context.SuKiens.FindAsync(suKienDto.Id);
        if (suKien == null)
        {
            throw new KeyNotFoundException("Không tìm thấy sự kiện");
        }
        suKien.Update(suKienDto.ThanhVienId, suKienDto.LoaiSuKien, suKienDto.NgayXayRa, suKienDto.DiaDiem, suKienDto.MoTa);
        await _context.SaveChangesAsync();
        return suKien;
    }
}