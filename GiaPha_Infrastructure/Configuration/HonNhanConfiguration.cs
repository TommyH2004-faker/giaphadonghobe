// filepath: d:\GiaPhaDongHo\GiaPha_Infrastructure\Configuration\HonNhanConfiguration.cs
using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class HonNhanConfiguration : IEntityTypeConfiguration<HonNhan>
{
    public void Configure(EntityTypeBuilder<HonNhan> builder)
    {
        builder.ToTable("HonNhans");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ChongId);

        builder.Property(x => x.VoId);

        builder.Property(x => x.NgayKetHon);

        builder.Property(x => x.NoiKetHon)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.Chong)
            .WithMany()
            .HasForeignKey(x => x.ChongId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vo)
            .WithMany()
            .HasForeignKey(x => x.VoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ChongId);
        builder.HasIndex(x => x.VoId);
    }
}
