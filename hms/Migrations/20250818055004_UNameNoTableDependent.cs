using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class UNameNoTableDependent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_unames",
                table: "unames");

            migrationBuilder.DropColumn(
                name: "table",
                table: "unames");

            migrationBuilder.AddPrimaryKey(
                name: "PK_unames",
                table: "unames",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_unames",
                table: "unames");

            migrationBuilder.AddColumn<string>(
                name: "table",
                table: "unames",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_unames",
                table: "unames",
                columns: new[] { "name", "table" });
        }
    }
}
