using GiaPha_Application.Common;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository;

public interface IHonNhanRepository
{
    Task<Result<HonNhan>> CreateAsync(HonNhan honNhan);
    Task<bool> ExistsAsync(Guid chongId, Guid voId);
    Task<bool> HasActiveMarriageAsync(Guid thanhVienId);
    Task<bool> IsBloodRelatedAsync(Guid nguoi1Id, Guid nguoi2Id);
    Task<Result<HonNhan>> GetByIdAsync(Guid id);
    Task<Result<HonNhan>> GetActiveMarriageAsync(Guid chongId, Guid voId);
    Task<Result<HonNhan>> GetActiveHonNhanByMemberId(Guid id);
}