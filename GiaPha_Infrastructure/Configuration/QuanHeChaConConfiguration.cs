using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class QuanHeChaConConfiguration : IEntityTypeConfiguration<QuanHeChaCon>
{
    public void Configure(EntityTypeBuilder<QuanHeChaCon> builder)
    {
        builder.ToTable("QuanHeChacons");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ChaMeId)
            ;

        builder.Property(x => x.ConId)
            ;

        builder.Property(x => x.LoaiQuanHe)
            ;

        // Relationships
        builder.HasOne(x => x.ChaMe)
            .WithMany()
            .HasForeignKey(x => x.ChaMeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Con)
            .WithMany()
            .HasForeignKey(x => x.ConId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ChaMeId);
        builder.HasIndex(x => x.ConId);
    }
}
