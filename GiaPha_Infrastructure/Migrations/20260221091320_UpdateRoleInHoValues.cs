using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleInHoValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update các giá trị cũ trong database
            // 1 (TruongHo cũ) → 0 (TruongHo mới)
            // 2 (ThanhVien cũ) → 1 (ThanhVien mới)
            migrationBuilder.Sql(@"
                UPDATE `TaiKhoan_Hos` 
                SET `RoleInHo` = CASE 
                    WHEN `RoleInHo` = 1 THEN 0
                    WHEN `RoleInHo` = 2 THEN 1
                    ELSE `RoleInHo`
                END
                WHERE `RoleInHo` IN (1, 2);
            ");

            migrationBuilder.AlterColumn<int>(
                name: "RoleInHo",
                table: "TaiKhoan_Hos",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RoleInHo",
                table: "TaiKhoan_Hos",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }
    }
}
