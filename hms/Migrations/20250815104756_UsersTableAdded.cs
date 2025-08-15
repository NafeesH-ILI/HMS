using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class UsersTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    uname = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    pass = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.uname);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "uname", "pass", "type" },
                values: new object[] { "sudo", "abcd", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
