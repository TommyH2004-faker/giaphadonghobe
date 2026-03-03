
using GiaPha_Application.DTOs;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository;    
public interface ISukienRepository
{
    // thêm , sửa , xoá , lấy hết sự kiện của các thành viên trong gia phả
    Task<List<SuKien>> GetEventsByThanhVienIdAsync(Guid thanhVienId);
    Task<SuKien> GetEventByIdAsync(Guid suKienId);
    Task<SuKien> CreateEventAsync(SuKien suKienDto);
    Task<SuKien> UpdateEventAsync(SuKien suKienDto);
    Task<bool> DeleteEventAsync(Guid suKienId);
    Task<List<SuKien>> GetUpcomingEventsAsync(int daysAhead);
    Task<List<SuKien>> GetAllAsync();
}