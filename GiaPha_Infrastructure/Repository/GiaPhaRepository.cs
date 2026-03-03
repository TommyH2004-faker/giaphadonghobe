using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using GiaPha_Application.Service;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GiaPha_Infrastructure.Repository;

public class GiaPhaRepository : IGiaPhaRepository
{
    private readonly DbGiaPha _context;
    private readonly GiaPhaTreeBuilder _treeBuilder;
    private readonly ILogger<GiaPhaRepository> _logger;

    public GiaPhaRepository(DbGiaPha context, GiaPhaTreeBuilder treeBuilder, ILogger<GiaPhaRepository> logger)
    {
        _context = context;
        _treeBuilder = treeBuilder;
        _logger = logger;
    }
    // ...existing code...
public async Task<Result<GiaPhaTreeResponse>> BuildGiaPhaTreeAsync(Guid hoId, int maxLevel = 10, bool includeNuGioi = true, bool includeDeleted = true)
{
    try
    {
        // 1. Load thông tin họ
        var ho = await _context.Hos
            .Include(h => h.ThuyTo)
            .FirstOrDefaultAsync(h => h.Id == hoId);

        if (ho?.ThuyTo == null)
        {
            _logger.LogWarning("Họ {HoId} hoặc thủy tổ không tồn tại", hoId);
            return Result<GiaPhaTreeResponse>.Failure(ErrorType.NotFound, "Họ hoặc thủy tổ không tồn tại");
        }

        _logger.LogInformation("Tìm thấy họ: {TenHo}, Thủy tổ: {ThuyTo}", ho.TenHo, ho.ThuyTo.HoTen);

        // 2. Load thành viên của họ (có thể bao gồm người đã xóa hoặc không)
        var membersQuery = _context.ThanhViens
            .Where(tv => tv.HoId == hoId);
        
        if (includeDeleted)
        {
            membersQuery = membersQuery.IgnoreQueryFilters(); // Load cả người đã xóa
        }
        
        var allMembers = await membersQuery
            .ToDictionaryAsync(tv => tv.Id, tv => tv);

        _logger.LogInformation("Tìm thấy {Count} thành viên trong họ", allMembers.Count);

        // 3. Load tất cả hôn nhân (chỉ của nam giới trong họ)
        var maleIds = allMembers.Values
            .Where(tv => !tv.GioiTinh) // Nam = false
            .Select(tv => tv.Id)
            .ToList();

        _logger.LogInformation("Có {Count} nam giới trong họ", maleIds.Count);

        var marriagesQuery = _context.HonNhans
            .Include(h => h.Vo)
            .Where(h => maleIds.Contains(h.ChongId));
        
        if (includeDeleted)
        {
            marriagesQuery = marriagesQuery.IgnoreQueryFilters(); // Load cả vợ đã xóa
        }
        
        var allMarriages = await marriagesQuery.ToListAsync();

        _logger.LogInformation("Tìm thấy {Count} hôn nhân", allMarriages.Count);

        var marriagesByHusband = allMarriages
            .GroupBy(h => h.ChongId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 🔧 FIX: Thêm tất cả vợ (từ họ khác) vào allMembers
        foreach (var marriage in allMarriages)
        {
            if (marriage.Vo != null && !allMembers.ContainsKey(marriage.Vo.Id))
            {
                allMembers[marriage.Vo.Id] = marriage.Vo;
                _logger.LogInformation("Thêm vợ từ họ khác: {VoName} (ID: {VoId})", 
                    marriage.Vo.HoTen, marriage.Vo.Id);
            }
        }

        // 4. Load TẤT CẢ quan hệ cha-mẹ-con (cả LoaiQuanHe = 0 và 1)
        var parentChildQuery = _context.QuanHeChaCons
            .Where(q => allMembers.Keys.Contains(q.ChaMeId) || allMembers.Keys.Contains(q.ConId));
        
        if (includeDeleted)
        {
            parentChildQuery = parentChildQuery.IgnoreQueryFilters(); // Load cả quan hệ của người đã xóa
        }
        
        var allParentChild = await parentChildQuery.ToListAsync(); // Bỏ filter LoaiQuanHe để lấy cả cha và mẹ

        _logger.LogInformation("Tìm thấy {Count} quan hệ cha-mẹ-con", allParentChild.Count);

        // Group theo cha (LoaiQuanHe = 0)
        var childrenByFather = allParentChild
            .Where(pc => pc.LoaiQuanHe == 0)
            .GroupBy(pc => pc.ChaMeId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ConId).ToList());

        // Group theo mẹ (LoaiQuanHe = 1)
        var childrenByMother = allParentChild
            .Where(pc => pc.LoaiQuanHe == 1)
            .GroupBy(pc => pc.ChaMeId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ConId).ToList());

        // Log chi tiết cho thủy tổ
        if (ho.ThuyToId.HasValue)
        {
            var thuyToId = ho.ThuyToId.Value;
            var hasMarriage = marriagesByHusband.ContainsKey(thuyToId);
            var hasChildren = childrenByFather.ContainsKey(thuyToId);
            
            _logger.LogInformation("Thủy tổ {ThuyToId}: Có hôn nhân={HasMarriage}, Có con={HasChildren}", 
                thuyToId, hasMarriage, hasChildren);

            if (hasMarriage)
            {
                var marriages = marriagesByHusband[thuyToId];
                _logger.LogInformation("Thủy tổ có {Count} hôn nhân", marriages.Count);
                foreach (var m in marriages)
                {
                    _logger.LogInformation("Hôn nhân với {VoName} (ID: {VoId})", m.Vo.HoTen, m.VoId);
                }
            }

            if (hasChildren)
            {
                var children = childrenByFather[thuyToId];
                _logger.LogInformation("Thủy tổ có {Count} con", children.Count);
            }
        }

        // 5. Xây dựng cây
        var tree = _treeBuilder.BuildTree(ho, allMembers, marriagesByHusband, childrenByFather, childrenByMother);

        _logger.LogInformation("Xây dựng cây thành công: {TongSoThanhVien} thành viên, {SoCapDo} cấp độ", 
            tree.TongSoThanhVien, tree.SoCapDo);

        return Result<GiaPhaTreeResponse>.Success(tree);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Lỗi xây dựng cây gia phả cho họ {HoId}", hoId);
        return Result<GiaPhaTreeResponse>.Failure(ErrorType.InternalError, $"Lỗi xây dựng cây gia phả: {ex.Message}");
    }
}
}