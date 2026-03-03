using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeThanhVienIdToHoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNguoiDungs_ThanhViens_ThanhVienId",
                table: "TaiKhoanNguoiDungs");

            migrationBuilder.RenameColumn(
                name: "ThanhVienId",
                table: "TaiKhoanNguoiDungs",
                newName: "HoId");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoanNguoiDungs_ThanhVienId",
                table: "TaiKhoanNguoiDungs",
                newName: "IX_TaiKhoanNguoiDungs_HoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNguoiDungs_Hos_HoId",
                table: "TaiKhoanNguoiDungs",
                column: "HoId",
                principalTable: "Hos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNguoiDungs_Hos_HoId",
                table: "TaiKhoanNguoiDungs");

            migrationBuilder.RenameColumn(
                name: "HoId",
                table: "TaiKhoanNguoiDungs",
                newName: "ThanhVienId");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoanNguoiDungs_HoId",
                table: "TaiKhoanNguoiDungs",
                newName: "IX_TaiKhoanNguoiDungs_ThanhVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNguoiDungs_ThanhViens_ThanhVienId",
                table: "TaiKhoanNguoiDungs",
                column: "ThanhVienId",
                principalTable: "ThanhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
