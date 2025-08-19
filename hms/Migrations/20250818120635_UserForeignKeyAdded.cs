using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class UserForeignKeyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UName",
                table: "patients",
                newName: "uname");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_users_uname",
                table: "doctors",
                column: "uname",
                principalTable: "users",
                principalColumn: "uname",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_users_uname",
                table: "patients",
                column: "uname",
                principalTable: "users",
                principalColumn: "uname",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_users_uname",
                table: "doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_users_uname",
                table: "patients");

            migrationBuilder.RenameColumn(
                name: "uname",
                table: "patients",
                newName: "UName");
        }
    }
}
