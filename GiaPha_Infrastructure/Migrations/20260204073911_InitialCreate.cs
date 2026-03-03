using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiaPha_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EntityName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChiHos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenChiHo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TruongChiId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiHos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dois",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SoDoi = table.Column<int>(type: "int", nullable: false),
                    TenDoi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dois", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HonNhans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ChongId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NgayKetHon = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NoiKetHon = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HonNhans", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Hos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenHo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HinhAnh = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayTao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    QueQuan = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThuyToId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ThanhViens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoTen = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GioiTinh = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NgayMat = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NoiSinh = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TieuSu = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrangThai = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ChiHoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DoiId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhViens_ChiHos_ChiHoId",
                        column: x => x.ChiHoId,
                        principalTable: "ChiHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThanhViens_Dois_DoiId",
                        column: x => x.DoiId,
                        principalTable: "Dois",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThanhViens_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MoPhans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ViTri = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KinhDo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ViDo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThanhVienId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoPhans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoPhans_ThanhViens_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NoiDung = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NguoiNhanId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsGlobal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ChiHoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    HoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DaDoc = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_ChiHos_ChiHoId",
                        column: x => x.ChiHoId,
                        principalTable: "ChiHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Hos_HoId",
                        column: x => x.HoId,
                        principalTable: "Hos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_ThanhViens_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuanHeChacons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ChaMeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ConId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LoaiQuanHe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHeChacons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuanHeChacons_ThanhViens_ChaMeId",
                        column: x => x.ChaMeId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuanHeChacons_ThanhViens_ConId",
                        column: x => x.ConId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SuKiens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ThanhVienId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LoaiSuKien = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayXayRa = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DiaDiem = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuKiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuKiens_ThanhViens_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaiKhoanNguoiDungs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenDangNhap = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MatKhau = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GioiTinh = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "User")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    ActivationCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ThanhVienId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNguoiDungs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNguoiDungs_ThanhViens_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TepTins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DuongDan = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThanhVienId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepTins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TepTins_ThanhViens_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ThanhTuus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ThanhVienId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenThanhTuu = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NamDatDuoc = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhTuus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhTuus_ThanhViens_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "ThanhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiHos_HoId",
                table: "ChiHos",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiHos_TruongChiId",
                table: "ChiHos",
                column: "TruongChiId");

            migrationBuilder.CreateIndex(
                name: "IX_Dois_HoId",
                table: "Dois",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonNhans_ChongId",
                table: "HonNhans",
                column: "ChongId");

            migrationBuilder.CreateIndex(
                name: "IX_HonNhans_VoId",
                table: "HonNhans",
                column: "VoId");

            migrationBuilder.CreateIndex(
                name: "IX_Hos_ThuyToId",
                table: "Hos",
                column: "ThuyToId");

            migrationBuilder.CreateIndex(
                name: "IX_MoPhans_ThanhVienId",
                table: "MoPhans",
                column: "ThanhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ChiHoId",
                table: "Notifications",
                column: "ChiHoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DaDoc",
                table: "Notifications",
                column: "DaDoc");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_HoId",
                table: "Notifications",
                column: "HoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NguoiNhanId",
                table: "Notifications",
                column: "NguoiNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeChacons_ChaMeId",
                table: "QuanHeChacons",
                column: "ChaMeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeChacons_ConId",
                table: "QuanHeChacons",
                column: "ConId");

            migrationBuilder.CreateIndex(
                name: "IX_SuKiens_ThanhVienId",
                table: "SuKiens",
                column: "ThanhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNguoiDungs_Email",
                table: "TaiKhoanNguoiDungs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNguoiDungs_TenDangNhap",
                table: "TaiKhoanNguoiDungs",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNguoiDungs_ThanhVienId",
                table: "TaiKhoanNguoiDungs",
                column: "ThanhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_TepTins_ThanhVienId",
                table: "TepTins",
                column: "ThanhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhTuus_ThanhVienId",
                table: "ThanhTuus",
                column: "ThanhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_ChiHoId",
                table: "ThanhViens",
                column: "ChiHoId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_DoiId",
                table: "ThanhViens",
                column: "DoiId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_HoId",
                table: "ThanhViens",
                column: "HoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiHos_Hos_HoId",
                table: "ChiHos",
                column: "HoId",
                principalTable: "Hos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiHos_ThanhViens_TruongChiId",
                table: "ChiHos",
                column: "TruongChiId",
                principalTable: "ThanhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dois_Hos_HoId",
                table: "Dois",
                column: "HoId",
                principalTable: "Hos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HonNhans_ThanhViens_ChongId",
                table: "HonNhans",
                column: "ChongId",
                principalTable: "ThanhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HonNhans_ThanhViens_VoId",
                table: "HonNhans",
                column: "VoId",
                principalTable: "ThanhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hos_ThanhViens_ThuyToId",
                table: "Hos",
                column: "ThuyToId",
                principalTable: "ThanhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiHos_Hos_HoId",
                table: "ChiHos");

            migrationBuilder.DropForeignKey(
                name: "FK_Dois_Hos_HoId",
                table: "Dois");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhViens_Hos_HoId",
                table: "ThanhViens");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiHos_ThanhViens_TruongChiId",
                table: "ChiHos");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "HonNhans");

            migrationBuilder.DropTable(
                name: "MoPhans");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "QuanHeChacons");

            migrationBuilder.DropTable(
                name: "SuKiens");

            migrationBuilder.DropTable(
                name: "TaiKhoanNguoiDungs");

            migrationBuilder.DropTable(
                name: "TepTins");

            migrationBuilder.DropTable(
                name: "ThanhTuus");

            migrationBuilder.DropTable(
                name: "Hos");

            migrationBuilder.DropTable(
                name: "ThanhViens");

            migrationBuilder.DropTable(
                name: "ChiHos");

            migrationBuilder.DropTable(
                name: "Dois");
        }
    }
}
