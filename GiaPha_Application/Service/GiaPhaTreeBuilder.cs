using GiaPha_Application.DTOs;
using GiaPha_Domain.Entities;

namespace GiaPha_Application.Service;

public class GiaPhaTreeBuilder
{
    public GiaPhaTreeResponse BuildTree(
        Ho ho,
        Dictionary<Guid, ThanhVien> allMembers,
        Dictionary<Guid, List<HonNhan>> marriagesByHusband,
        Dictionary<Guid, List<Guid>> childrenByFather,
        Dictionary<Guid, List<Guid>> childrenByMother)
    {
        if (ho.ThuyTo == null || !ho.ThuyToId.HasValue)
            throw new ArgumentException("Thủy tổ không tồn tại");

        var processedNodes = new Dictionary<Guid, GiaPhaNodeDto>();
        var queue = new Queue<(Guid memberId, int level)>();

        queue.Enqueue((ho.ThuyToId.Value, 0));
        GiaPhaNodeDto? rootNode = null;

        while (queue.Count > 0)
        {
            var (memberId, level) = queue.Dequeue();

            if (!allMembers.TryGetValue(memberId, out var member))
                continue;

            // ============================
            // GET OR CREATE NODE
            // ============================
            if (!processedNodes.TryGetValue(memberId, out var node))
            {
                node = CreateNode(member, level);
                processedNodes[memberId] = node;
            }

            if (level == 0)
                rootNode = node;

            // ============================
            // HÔN NHÂN (NAM)
            // ============================
            if (!member.GioiTinh &&
                marriagesByHusband.TryGetValue(memberId, out var marriages))
            {
                foreach (var marriage in marriages)
                {
                    var spouse = allMembers.GetValueOrDefault(marriage.VoId);
                    if (spouse == null) continue;

                    if (node.DanhSachVoChong
                        .Any(v => v.VoChongId == spouse.Id))
                        continue;

                    var voChongDto = CreateVoChongDto(marriage, spouse);

                    var children = GetChildrenOfMarriage(
                        marriage,
                        childrenByFather,
                        childrenByMother,
                        allMembers);

                    voChongDto.ConIds = children
                        .Select(c => c.Id)
                        .ToList();

                    node.DanhSachVoChong.Add(voChongDto);

                    // enqueue con
                    foreach (var child in children)
                    {
                        if (!processedNodes.ContainsKey(child.Id))
                            queue.Enqueue((child.Id, level + 1));
                    }
                }
            }

            // ============================
            // FALLBACK CHA -> CON
            // ============================
            if (childrenByFather.TryGetValue(memberId, out var fatherChildren))
            {
                foreach (var childId in fatherChildren)
                {
                    if (!processedNodes.ContainsKey(childId))
                        queue.Enqueue((childId, level + 1));
                }
            }

            // ============================
            // METADATA
            // ============================
            node.TongSoCon = node.DanhSachVoChong
                .SelectMany(v => v.ConIds)
                .Distinct()
                .Count();

            node.HasChildren = node.TongSoCon > 0;
        }

        BuildChildRelationships(processedNodes);

        return new GiaPhaTreeResponse
        {
            TenHo = ho.TenHo,
            HoId = ho.Id,
            ThuyTo = rootNode!,
            TongSoThanhVien = processedNodes.Count,
            SoCapDo = processedNodes.Values.Max(n => n.Level) + 1
        };
    }

    // ===================================================
    // HELPERS
    // ===================================================

    private GiaPhaNodeDto CreateNode(ThanhVien member, int level)
    {
        return new GiaPhaNodeDto
        {
            Id = member.Id,
            HoTen = member.HoTen,
            GioiTinh = member.GioiTinh,
            NgaySinh = member.NgaySinh,
            NgayMat = member.NgayMat,
            Level = level,
            Avatar = member.Avatar,
            TieuSu = member.TieuSu, // ✅ Map Tiểu sử
            HoId = member.HoId ?? Guid.Empty,
            IsDeleted = member.IsDeleted // ✅ Truyền thông tin IsDeleted
        };
    }

    private VoChongDto CreateVoChongDto(HonNhan marriage, ThanhVien spouse)
    {
        return new VoChongDto
        {
            HonNhanId = marriage.Id,
            VoChongId = spouse.Id,
            HoTen = spouse.HoTen,
            NgaySinh = spouse.NgaySinh,
            NgayMat = spouse.NgayMat,
            Avatar = spouse.Avatar,
            TieuSu = spouse.TieuSu, // ✅ Map Tiểu sử của vợ/chồng
            NgayKetHon = marriage.NgayKetHon ?? DateTime.MinValue,
            NgayLyHon = marriage.NgayLyHon,
            TrangThaiHonNhan = marriage.TrangThai,
            IsDeleted = spouse.IsDeleted // ✅ Truyền thông tin IsDeleted
        };
    }

    private List<ThanhVien> GetChildrenOfMarriage(
        HonNhan marriage,
        Dictionary<Guid, List<Guid>> childrenByFather,
        Dictionary<Guid, List<Guid>> childrenByMother,
        Dictionary<Guid, ThanhVien> allMembers)
    {
        var result = new List<ThanhVien>();

        if (childrenByFather.TryGetValue(marriage.ChongId, out var fatherChildren))
        {
            foreach (var childId in fatherChildren)
            {
                if (allMembers.TryGetValue(childId, out var child))
                    result.Add(child);
            }
        }

        return result
            .OrderBy(c => c.NgaySinh)
            .ToList();
    }

    private void BuildChildRelationships(
        Dictionary<Guid, GiaPhaNodeDto> processedNodes)
    {
        foreach (var node in processedNodes.Values)
        {
            foreach (var marriage in node.DanhSachVoChong)
            {
                foreach (var childId in marriage.ConIds)
                {
                    if (processedNodes.TryGetValue(childId, out var childNode))
                    {
                        if (!node.Con.Any(c => c.Id == childId))
                            node.Con.Add(childNode);
                    }
                }
            }
        }
    }
}
