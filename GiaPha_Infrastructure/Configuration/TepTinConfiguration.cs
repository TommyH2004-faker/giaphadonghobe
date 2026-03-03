using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class TepTinConfiguration : IEntityTypeConfiguration<TepTin>
{
    public void Configure(EntityTypeBuilder<TepTin> builder)
    {
        builder.ToTable("TepTins");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DuongDan)
        
            .HasMaxLength(1000);

        builder.Property(x => x.MoTa)
            .HasMaxLength(500);

        builder.Property(x => x.ThanhVienId)
            ;

        // Relationships
        builder.HasOne(x => x.ThanhVien)
            .WithMany()
            .HasForeignKey(x => x.ThanhVienId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.ThanhVienId);
    }
}
