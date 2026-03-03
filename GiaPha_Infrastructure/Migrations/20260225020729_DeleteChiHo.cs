using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteChiHo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ChiHos_ChiHoId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhViens_ChiHos_ChiHoId",
                table: "ThanhViens");

            migrationBuilder.DropTable(
                name: "ChiHos");

            migrationBuilder.DropIndex(
                name: "IX_ThanhViens_ChiHoId",
                table: "ThanhViens");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ChiHoId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ChiHoId",
                table: "ThanhViens");

            migrationBuilder.DropColumn(
                name: "ChiHoId",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChiHoId",
                table: "ThanhViens",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ChiHoId",
                table: "Notifications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "ChiHos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TruongChiId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenChiHo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiHos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiHos_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiHos_ThanhViens_TruongChiId",
                        column: x => x.TruongChiId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_ChiHoId",
                table: "ThanhViens",
                column: "ChiHoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ChiHoId",
                table: "Notifications",
                column: "ChiHoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiHos_HoId",
                table: "ChiHos",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiHos_TruongChiId",
                table: "ChiHos",
                column: "TruongChiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ChiHos_ChiHoId",
                table: "Notifications",
                column: "ChiHoId",
                principalTable: "ChiHos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhViens_ChiHos_ChiHoId",
                table: "ThanhViens",
                column: "ChiHoId",
                principalTable: "ChiHos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
