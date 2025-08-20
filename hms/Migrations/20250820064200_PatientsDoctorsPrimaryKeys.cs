using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class PatientsDoctorsPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_AspNetUsers_uname",
                table: "doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_AspNetUsers_uname",
                table: "patients");

            migrationBuilder.RenameColumn(
                name: "uname",
                table: "patients",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "uname",
                table: "doctors",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_AspNetUsers_id",
                table: "doctors",
                column: "id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_AspNetUsers_id",
                table: "patients",
                column: "id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_AspNetUsers_id",
                table: "doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_AspNetUsers_id",
                table: "patients");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "patients",
                newName: "uname");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "doctors",
                newName: "uname");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_AspNetUsers_uname",
                table: "doctors",
                column: "uname",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_AspNetUsers_uname",
                table: "patients",
                column: "uname",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
