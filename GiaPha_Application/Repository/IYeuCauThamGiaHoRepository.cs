using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository;

public interface IYeuCauThamGiaHoRepository
{
    Task AddAsync(YeuCauThamGiaHo yeuCau);
    Task<YeuCauThamGiaHo?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<YeuCauThamGiaHo>> GetPendingByHoIdAsync(Guid hoId);
    Task<bool> ExistsPendingAsync(Guid userId, Guid hoId);
}
