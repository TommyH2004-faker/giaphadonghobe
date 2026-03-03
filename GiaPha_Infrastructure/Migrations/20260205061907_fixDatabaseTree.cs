using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixDatabaseTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhViens_Dois_DoiId",
                table: "ThanhViens");

            migrationBuilder.DropTable(
                name: "Dois");

            migrationBuilder.DropIndex(
                name: "IX_ThanhViens_DoiId",
                table: "ThanhViens");

            migrationBuilder.DropColumn(
                name: "DoiId",
                table: "ThanhViens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoiId",
                table: "ThanhViens",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Dois",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SoDoi = table.Column<int>(type: "int", nullable: false),
                    TenDoi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dois", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dois_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_DoiId",
                table: "ThanhViens",
                column: "DoiId");

            migrationBuilder.CreateIndex(
                name: "IX_Dois_HoId",
                table: "Dois",
                column: "HoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhViens_Dois_DoiId",
                table: "ThanhViens",
                column: "DoiId",
                principalTable: "Dois",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
