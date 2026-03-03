using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository;
public interface IHoRepository
{
    Task<Result<Ho?>> CreateHoAsync(string tenHo, string? moTa , string? queQuan);
    Task<Result<bool>> DeleteHoAsync(Guid id);
    Task<Result<IEnumerable<Ho>>> GetAllHoAsync();
    Task<Result<Ho?>> GetHoByIdAsync(Guid hoId);
    Task<Result<Ho?>> GetHoByNameAsync(string tenHo);
    Task<Result<List<Ho>>> GetHosByThanhVienIdAsync(Guid thanhVienId);
    Task<Result<List<Ho>>> GetHosByUserIdAsync(Guid userId);
    Task<Result<List<Ho>>> GetTop3HoAsync();
    Task<Result<Ho>> UpdateHoAsync(Ho ho);
    Task<Result<bool>> AddUserToHoAsync(Guid userId, Guid hoId, RoleCuaHo role);
    Task<Result<string?>> GetTruongHoEmailAsync(Guid hoId);
}