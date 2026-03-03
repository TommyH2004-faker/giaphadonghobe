using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class ThanhTuuConfiguration : IEntityTypeConfiguration<ThanhTuu>
{
    public void Configure(EntityTypeBuilder<ThanhTuu> builder)
    {
        builder.ToTable("ThanhTuus");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ThanhVienId)
      ;

        builder.Property(x => x.TenThanhTuu)
          
            .HasMaxLength(255);

        builder.Property(x => x.NamDatDuoc);

        builder.Property(x => x.MoTa)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(x => x.ThanhVien)
            .WithMany()
            .HasForeignKey(x => x.ThanhVienId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.ThanhVienId);
    }
}
