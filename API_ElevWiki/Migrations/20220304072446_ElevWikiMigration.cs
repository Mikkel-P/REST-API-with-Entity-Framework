using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_ElevWiki.Migrations
{
    public partial class ElevWikiMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    studentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.studentId);
                });

            migrationBuilder.CreateTable(
                name: "Abscences",
                columns: table => new
                {
                    abscenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    legalAbscence = table.Column<bool>(type: "bit", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abscences", x => x.abscenceId);
                    table.ForeignKey(
                        name: "FK_Abscences_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actives",
                columns: table => new
                {
                    activeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    activeStatus = table.Column<bool>(type: "bit", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actives", x => x.activeId);
                    table.ForeignKey(
                        name: "FK_Actives_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoginHolders",
                columns: table => new
                {
                    loginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    confirmedEmail = table.Column<bool>(type: "bit", nullable: false),
                    admin = table.Column<bool>(type: "bit", nullable: false),
                    confirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHolders", x => x.loginId);
                    table.ForeignKey(
                        name: "FK_LoginHolders_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NfcCards",
                columns: table => new
                {
                    nfcId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nfcTag = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NfcCards", x => x.nfcId);
                    table.ForeignKey(
                        name: "FK_NfcCards_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeRecords",
                columns: table => new
                {
                    recId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    timestamp = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    checkStatus = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRecords", x => x.recId);
                    table.ForeignKey(
                        name: "FK_TimeRecords_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abscences_studentId",
                table: "Abscences",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_Actives_studentId",
                table: "Actives",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHolders_studentId",
                table: "LoginHolders",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_NfcCards_studentId",
                table: "NfcCards",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRecords_studentId",
                table: "TimeRecords",
                column: "studentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abscences");

            migrationBuilder.DropTable(
                name: "Actives");

            migrationBuilder.DropTable(
                name: "LoginHolders");

            migrationBuilder.DropTable(
                name: "NfcCards");

            migrationBuilder.DropTable(
                name: "TimeRecords");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
