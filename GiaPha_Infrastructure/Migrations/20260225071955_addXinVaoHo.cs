using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addXinVaoHo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YeuCauThamGiaHos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LyDoXinVao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NguoiXuLyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    GhiChuTuChoi = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauThamGiaHos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCauThamGiaHos_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YeuCauThamGiaHos_TaiKhoanNguoiDungs_NguoiXuLyId",
                        column: x => x.NguoiXuLyId,
                        principalTable: "TaiKhoanNguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauThamGiaHos_TaiKhoanNguoiDungs_UserId",
                        column: x => x.UserId,
                        principalTable: "TaiKhoanNguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThamGiaHos_HoId",
                table: "YeuCauThamGiaHos",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThamGiaHos_NguoiXuLyId",
                table: "YeuCauThamGiaHos",
                column: "NguoiXuLyId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThamGiaHos_TrangThai",
                table: "YeuCauThamGiaHos",
                column: "TrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThamGiaHos_UserId",
                table: "YeuCauThamGiaHos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThamGiaHos_UserId_HoId_TrangThai",
                table: "YeuCauThamGiaHos",
                columns: new[] { "UserId", "HoId", "TrangThai" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YeuCauThamGiaHos");
        }
    }
}
