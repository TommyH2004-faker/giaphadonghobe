using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateTo_TaiKhoan_Ho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK nếu tồn tại
            migrationBuilder.Sql(@"
                SET @fkExists = (SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'TaiKhoanNguoiDungs' 
                    AND CONSTRAINT_NAME = 'FK_TaiKhoanNguoiDungs_Hos_HoId');
                
                SET @sqlDropFK = IF(@fkExists > 0, 
                    'ALTER TABLE `TaiKhoanNguoiDungs` DROP FOREIGN KEY `FK_TaiKhoanNguoiDungs_Hos_HoId`', 
                    'SELECT ''FK does not exist''');
                    
                PREPARE stmt FROM @sqlDropFK;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Drop Index nếu tồn tại
            migrationBuilder.Sql(@"
                SET @idxExists = (SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.STATISTICS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'TaiKhoanNguoiDungs' 
                    AND INDEX_NAME = 'IX_TaiKhoanNguoiDungs_HoId');
                
                SET @sqlDropIdx = IF(@idxExists > 0, 
                    'ALTER TABLE `TaiKhoanNguoiDungs` DROP INDEX `IX_TaiKhoanNguoiDungs_HoId`', 
                    'SELECT ''Index does not exist''');
                    
                PREPARE stmt FROM @sqlDropIdx;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Drop Column nếu tồn tại
            migrationBuilder.Sql(@"
                SET @colExists = (SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'TaiKhoanNguoiDungs' 
                    AND COLUMN_NAME = 'HoId');
                
                SET @sqlDropCol = IF(@colExists > 0, 
                    'ALTER TABLE `TaiKhoanNguoiDungs` DROP COLUMN `HoId`', 
                    'SELECT ''Column does not exist''');
                    
                PREPARE stmt FROM @sqlDropCol;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.CreateTable(
                name: "TaiKhoan_Hos",
                columns: table => new
                {
                    TaiKhoanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleInHo = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    NgayThamGia = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan_Hos", x => new { x.TaiKhoanId, x.HoId });
                    table.ForeignKey(
                        name: "FK_TaiKhoan_Hos_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_Hos_TaiKhoanNguoiDungs_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoanNguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Hos_HoId",
                table: "TaiKhoan_Hos",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Hos_TaiKhoanId",
                table: "TaiKhoan_Hos",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Hos_TaiKhoanId_RoleInHo",
                table: "TaiKhoan_Hos",
                columns: new[] { "TaiKhoanId", "RoleInHo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoan_Hos");

            migrationBuilder.AddColumn<Guid>(
                name: "HoId",
                table: "TaiKhoanNguoiDungs",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNguoiDungs_HoId",
                table: "TaiKhoanNguoiDungs",
                column: "HoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNguoiDungs_Hos_HoId",
                table: "TaiKhoanNguoiDungs",
                column: "HoId",
                principalTable: "Hos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
