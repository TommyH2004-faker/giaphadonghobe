using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;
public class ThanhVienRepository : IThanhVienRepository
{
    private readonly DbGiaPha _context;
    public ThanhVienRepository(DbGiaPha context)
    {
        _context = context;
    }

    public async Task<Result<ThanhVien?>> CreateThanhVienAsync(ThanhVien thanhVien)
    {
        _context.ThanhViens.Add(thanhVien);
        return Result<ThanhVien?>.Success(thanhVien);
    }

    // public async Task<Result<bool>> DeleteThanhVienAsync(Guid id)
    // {
    //     var thanhVien = await _context.ThanhViens.FindAsync(id);
    //     if (thanhVien == null)
    //     {
    //         return Result<bool>.Failure(ErrorType.NotFound, "Thành viên không tồn tại");
    //     }
    //     _context.ThanhViens.Remove(thanhVien);
    //     return Result<bool>.Success(true);
    // }
    public async Task<Result<bool>> DeleteThanhVienAsync(Guid id)
    {
        // ✅ SOFT DELETE: Chỉ đánh dấu IsDeleted = true
        var entity = await _context.ThanhViens
            .IgnoreQueryFilters() // Bỏ qua query filter để có thể tìm cả người đã xóa
            .FirstOrDefaultAsync(tv => tv.Id == id);
            
        if (entity == null)
            return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");

        // Kiểm tra đã bị xóa chưa
        if (entity.IsDeleted)
            return Result<bool>.Failure(ErrorType.Conflict, "Thành viên đã bị xóa trước đó");

        // Gọi method Delete() của entity để đánh dấu IsDeleted = true
        entity.Delete();

        // EF Core sẽ tự động track và update
        _context.ThanhViens.Update(entity);

        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> CascadeDeleteThanhVienAsync(Guid id)
    {
        // 🔥 CASCADE SOFT DELETE: Xóa người này + TẤT CẢ con cháu + VỢ/CHỒNG (bao gồm cả vợ/chồng của con cháu)
        var entity = await _context.ThanhViens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(tv => tv.Id == id);
            
        if (entity == null)
            return Result<int>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");

        if (entity.IsDeleted)
            return Result<int>.Failure(ErrorType.Conflict, "Thành viên đã bị xóa trước đó");

        // 1️⃣ Đánh dấu IsDeleted cho người này
        entity.Delete();
        _context.ThanhViens.Update(entity);
        int deletedCount = 1;

        // 2️⃣ Xóa TẤT CẢ vợ/chồng của người này
        var spouseIds = await GetSpouseIdsAsync(id);
        foreach (var spouseId in spouseIds)
        {
            var spouse = await _context.ThanhViens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(tv => tv.Id == spouseId);
                
            if (spouse != null && !spouse.IsDeleted)
            {
                spouse.Delete();
                _context.ThanhViens.Update(spouse);
                deletedCount++;
            }
        }

        // 3️⃣ Tìm TẤT CẢ con cháu (recursive)
        var descendants = await GetAllDescendantsAsync(id);
        
        foreach (var descendant in descendants)
        {
            if (!descendant.IsDeleted)
            {
                descendant.Delete();
                _context.ThanhViens.Update(descendant);
                deletedCount++;
                
                // 🔥 QUAN TRỌNG: Xóa luôn vợ/chồng của mỗi con cháu
                var descendantSpouseIds = await GetSpouseIdsAsync(descendant.Id);
                foreach (var spouseId in descendantSpouseIds)
                {
                    var spouse = await _context.ThanhViens
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(tv => tv.Id == spouseId);
                        
                    if (spouse != null && !spouse.IsDeleted)
                    {
                        spouse.Delete();
                        _context.ThanhViens.Update(spouse);
                        deletedCount++;
                    }
                }
            }
        }

        return Result<int>.Success(deletedCount);
    }

    private async Task<List<Guid>> GetSpouseIdsAsync(Guid memberId)
    {
        // Lấy tất cả vợ (nếu là chồng)
        var wifeIds = await _context.HonNhans
            .Where(h => h.ChongId == memberId)
            .Select(h => h.VoId)
            .ToListAsync();

        // Lấy tất cả chồng (nếu là vợ)
        var husbandIds = await _context.HonNhans
            .Where(h => h.VoId == memberId)
            .Select(h => h.ChongId)
            .ToListAsync();

        return wifeIds.Concat(husbandIds).Distinct().ToList();
    }

    private async Task<List<GiaPha_Domain.Entities.ThanhVien>> GetAllDescendantsAsync(Guid parentId)
    {
        var descendants = new List<GiaPha_Domain.Entities.ThanhVien>();
        var queue = new Queue<Guid>();
        queue.Enqueue(parentId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            // Lấy tất cả con (cả cha và mẹ)
            var childIds = await _context.QuanHeChaCons
                .Where(qh => qh.ChaMeId == currentId)
                .Select(qh => qh.ConId)
                .Distinct()
                .ToListAsync();

            foreach (var childId in childIds)
            {
                var child = await _context.ThanhViens
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(tv => tv.Id == childId);

                if (child != null && !descendants.Any(d => d.Id == child.Id))
                {
                    descendants.Add(child);
                    queue.Enqueue(childId); // Tiếp tục tìm con của con
                }
            }
        }

        return descendants;
    }
    public async  Task<Result<IEnumerable<ThanhVien>>> GetAllThanhVienAsync()
    {
        var thanhViens = await _context.ThanhViens.ToListAsync();
        return Result<IEnumerable<ThanhVien>>.Success(thanhViens);
    }

    public async Task<Result<ThanhVien?>> GetThanhVienByIdAsync(Guid thanhVienId)
    {
        // Sử dụng FirstOrDefaultAsync để query filter được áp dụng (tự động lọc IsDeleted = true)
        var thanhVien = await _context.ThanhViens
            .FirstOrDefaultAsync(tv => tv.Id == thanhVienId);
        return Result<ThanhVien?>.Success(thanhVien);
    }

    public async Task<Result<ThanhVien?>> GetThanhVienByIdWithDeletedAsync(Guid thanhVienId)
    {
        // ⚠️ Dùng IgnoreQueryFilters để có thể tìm cả người đã xóa
        var thanhVien = await _context.ThanhViens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(tv => tv.Id == thanhVienId);
        return Result<ThanhVien?>.Success(thanhVien);
    }

    public async Task<Result<ThanhVien?>> GetThanhVienByNameAsync(string hoTen)
    {
        var thanhVien = await _context.ThanhViens.FirstOrDefaultAsync(t => t.HoTen == hoTen);
        return Result<ThanhVien?>.Success(thanhVien);
    }

    public async Task<Result<List<ThanhVien>>> GetThanhVienByEmailAsync(string email)
    {
       throw new NotImplementedException(); 
    }




    public async Task<Result<ThanhVien>> UpdateThanhVienAsync(ThanhVien thanhVien)
    {
        var existingThanhVien = await _context.ThanhViens.FindAsync(thanhVien.Id);
        if (existingThanhVien == null)
        {
            return Result<ThanhVien>.Failure(ErrorType.NotFound, "Thành viên không tồn tại");
        }
        _context.ThanhViens.Update(thanhVien);
        return Result<ThanhVien>.Success(thanhVien);
    }

    public async Task<Result<ThanhVien?>> GetHoById(Guid conId)
    {
        var thanhVien = await _context.ThanhViens.FindAsync(conId);
        return Result<ThanhVien?>.Success(thanhVien);
    }

    public async Task<List<ThanhVien>> GetThanhVienByHoId(Guid hoId)
    {
        var thanhViens = await _context.ThanhViens
            .Where(tv => tv.HoId == hoId)
            .ToListAsync();
        return thanhViens;
    }
}