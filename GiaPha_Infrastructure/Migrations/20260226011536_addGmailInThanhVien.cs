using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addGmailInThanhVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gmail",
                table: "ThanhViens",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gmail",
                table: "ThanhViens");
        }
    }
}
