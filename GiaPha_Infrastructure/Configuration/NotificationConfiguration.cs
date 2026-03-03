using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.NoiDung)
        
            .HasMaxLength(2000);

        builder.Property(x => x.NguoiNhanId);

        builder.Property(x => x.IsGlobal);

        builder.Property(x => x.HoId);

        builder.Property(x => x.DaDoc)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt);

        // Relationships
        builder.HasOne(x => x.NguoiNhan)
            .WithMany()
            .HasForeignKey(x => x.NguoiNhanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Ho)
            .WithMany()
            .HasForeignKey(x => x.HoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.NguoiNhanId);
        builder.HasIndex(x => x.HoId);
        builder.HasIndex(x => x.DaDoc);
    }
}
