using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.EntityName)
            .HasMaxLength(255);

        builder.Property(x => x.EntityId);


        builder.Property(x => x.Action)
            .HasMaxLength(50);

        builder.Property(x => x.ChangedBy)
            .HasMaxLength(255);

        builder.Property(x => x.ChangedAt);

        builder.Property(x => x.OldValues)
            .HasColumnType("TEXT");

        builder.Property(x => x.NewValues)
            .HasColumnType("TEXT");

        // Indexes
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.ChangedAt);
    }
}
