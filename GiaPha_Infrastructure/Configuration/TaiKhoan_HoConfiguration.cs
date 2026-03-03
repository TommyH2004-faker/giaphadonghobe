using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class TaiKhoan_HoConfiguration : IEntityTypeConfiguration<TaiKhoan_Ho>
{
    public void Configure(EntityTypeBuilder<TaiKhoan_Ho> builder)
    {
        builder.ToTable("TaiKhoan_Hos");
        
        // Composite Primary Key
        builder.HasKey(x => new { x.TaiKhoanId, x.HoId });

        // Properties
        builder.Property(x => x.RoleInHo)
            .IsRequired()
            .HasConversion<int>() // Lưu enum dưới dạng int: 0 = TruongHo, 1 = ThanhVien
            .HasDefaultValue(RoleCuaHo.ThanhVien); // Default = 1 (Thành viên)

        builder.Property(x => x.NgayThamGia);
        // Relationships
        builder.HasOne(x => x.TaiKhoan)
            .WithMany(t => t.TaiKhoan_Hos)
            .HasForeignKey(x => x.TaiKhoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Ho)
            .WithMany(h => h.TaiKhoan_Hos)
            .HasForeignKey(x => x.HoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.TaiKhoanId);
        builder.HasIndex(x => x.HoId);
        builder.HasIndex(x => new { x.TaiKhoanId, x.RoleInHo });
    }
}
