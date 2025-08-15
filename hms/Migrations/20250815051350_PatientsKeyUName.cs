using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class PatientsKeyUName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    uname = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.uname);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    UName = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    dob = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.UName);
                });

            migrationBuilder.CreateTable(
                name: "unames",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    table = table.Column<string>(type: "text", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unames", x => new { x.name, x.table });
                });

            migrationBuilder.CreateTable(
                name: "doctors",
                columns: table => new
                {
                    uname = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    max_qual = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    specialization = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dept = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors", x => x.uname);
                    table.ForeignKey(
                        name: "FK_doctors_departments_dept",
                        column: x => x.dept,
                        principalTable: "departments",
                        principalColumn: "uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctors_dept",
                table: "doctors",
                column: "dept");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctors");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "unames");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
