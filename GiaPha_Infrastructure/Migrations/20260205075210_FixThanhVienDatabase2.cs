using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixThanhVienDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayLyHon",
                table: "HonNhans",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "HonNhans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayLyHon",
                table: "HonNhans");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "HonNhans");
        }
    }
}
