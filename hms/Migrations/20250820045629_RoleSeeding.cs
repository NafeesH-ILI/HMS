using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class RoleSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "64a1c0ab-8783-4cb7-ad7d-254c050815ba", null, "Admin", "ADMIN" },
                    { "64a1c0ab-8783-4cb7-ad7d-254c050815ca", null, "Receptionist", "RECEPTIONIST" },
                    { "64a1c0ab-8783-4cb7-ad7d-254c050815da", null, "Doctor", "DOCTOR" },
                    { "64a1c0ab-8783-4cb7-ad7d-254c050815ea", null, "Patient", "PATIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64a1c0ab-8783-4cb7-ad7d-254c050815ba");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64a1c0ab-8783-4cb7-ad7d-254c050815ca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64a1c0ab-8783-4cb7-ad7d-254c050815da");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64a1c0ab-8783-4cb7-ad7d-254c050815ea");
        }
    }
}
