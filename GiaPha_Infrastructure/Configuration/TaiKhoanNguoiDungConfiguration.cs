using GiaPha_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiaPha_Infrastructure.Configuration;

public class TaiKhoanNguoiDungConfiguration : IEntityTypeConfiguration<TaiKhoanNguoiDung>
{
    public void Configure(EntityTypeBuilder<TaiKhoanNguoiDung> builder)
    {
        builder.ToTable("TaiKhoanNguoiDungs");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TenDangNhap)
            
            .HasMaxLength(100);

        builder.Property(x => x.MatKhau)
          
            .HasMaxLength(255);

        builder.Property(x => x.Email)
        
            .HasMaxLength(255);

        builder.Property(x => x.Avatar)
            .HasMaxLength(500);

        builder.Property(x => x.GioiTinh)
            ;

        builder.Property(x => x.SoDienThoai)
            .HasMaxLength(20);

        builder.Property(x => x.Role)
      
            .HasMaxLength(50)
            .HasDefaultValue("User");

        builder.Property(x => x.Enabled)
         
            .HasDefaultValue(false);

        builder.Property(x => x.ActivationCode)
            .HasMaxLength(10);

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);

        builder.Property(x => x.RefreshTokenExpiry);

        // Many-to-Many với Ho qua TaiKhoan_Ho được config ở TaiKhoan_HoConfiguration

        // Indexes
        builder.HasIndex(x => x.TenDangNhap)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}
