using GiaPha_Application.Common;
using GiaPha_Application.DTOs;

namespace GiaPha_Application.Repository;

public interface IGiaPhaRepository
{
    Task<Result<GiaPhaTreeResponse>> BuildGiaPhaTreeAsync(Guid hoId, int maxLevel = 10, bool includeNuGioi = true, bool includeDeleted = true);
}