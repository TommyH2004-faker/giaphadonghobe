
using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GiaPha_Infrastructure.Configuration;
public class SuKienConfiguration : IEntityTypeConfiguration<SuKien>
{
    public void Configure(EntityTypeBuilder<SuKien> builder)
    {
        builder.ToTable("SuKiens");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ThanhVienId)
          ;

        builder.Property(x => x.LoaiSuKien)
          
            .HasMaxLength(100);

        builder.Property(x => x.NgayXayRa)
            ;

        builder.Property(x => x.DiaDiem)
            .HasMaxLength(500);

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