using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class ThanhVienConfiguration : IEntityTypeConfiguration<ThanhVien>
{
    public void Configure(EntityTypeBuilder<ThanhVien> builder)
    {
        builder.ToTable("ThanhViens");
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.HoTen)
        
            .HasMaxLength(255);

        builder.Property(x => x.GioiTinh)
          ;
            builder.Property(x => x.Gmail)
                .HasMaxLength(255);

        builder.Property(x => x.NgaySinh);

        builder.Property(x => x.NgayMat);

        builder.Property(x => x.NoiSinh)
            .HasMaxLength(500);

        builder.Property(x => x.TieuSu)
            .HasMaxLength(2000);

        builder.Property(x => x.TrangThai)
       
            .HasDefaultValue(1);

        builder.Property(x => x.HoId)
            .IsRequired(false); // Nullable: vợ/chồng có thể không thuộc họ

        // Relationships
        builder.HasOne(x => x.Ho)
            .WithMany(h => h.ThanhViens)
            .HasForeignKey(x => x.HoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.HoId);

    }
}
