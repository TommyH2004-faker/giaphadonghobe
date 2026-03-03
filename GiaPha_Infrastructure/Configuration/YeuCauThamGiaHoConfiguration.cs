using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class YeuCauThamGiaHoConfiguration : IEntityTypeConfiguration<YeuCauThamGiaHo>
{
    public void Configure(EntityTypeBuilder<YeuCauThamGiaHo> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LyDoXinVao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .IsRequired();

        builder.Property(x => x.GhiChuTuChoi)
            .HasMaxLength(1000);

        // Index
        builder.HasIndex(x => x.TrangThai);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.HoId);
        builder.HasIndex(x => new { x.UserId, x.HoId, x.TrangThai });

        // FK User
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK Ho
        builder.HasOne(x => x.Ho)
            .WithMany()
            .HasForeignKey(x => x.HoId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK NguoiXuLy
        builder.HasOne(x => x.NguoiXuLy)
            .WithMany()
            .HasForeignKey(x => x.NguoiXuLyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
