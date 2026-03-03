using GiaPha_Application.Common;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository;
public interface IQuanHeChaMeRepository
{
    Task<Result<QuanHeChaCon>> CreateAsync(QuanHeChaCon quanHe);
    Task<bool> ExistsAsync(Guid chaMeId, Guid conId, int loaiQuanHe);
    Task<int> CountParentAsync(Guid conId, int loaiQuanHe);
    Task<bool> IsLoopAsync(Guid chaMeId, Guid conId);
}