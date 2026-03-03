using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class MoPhanConfiguration : IEntityTypeConfiguration<MoPhan>
{
    public void Configure(EntityTypeBuilder<MoPhan> builder)
    {
        builder.ToTable("MoPhans");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ViTri)
            .HasMaxLength(500);

        builder.Property(x => x.KinhDo)
            .HasMaxLength(50);

        builder.Property(x => x.ViDo)
            .HasMaxLength(50);

        builder.Property(x => x.MoTa)
            .HasMaxLength(1000);

        builder.Property(x => x.ThanhVienId)
          ;

        // Relationships
        builder.HasOne(x => x.ThanhVien)
            .WithMany()
            .HasForeignKey(x => x.ThanhVienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}