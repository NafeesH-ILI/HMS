using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hms.Migrations
{
    /// <inheritdoc />
    public partial class NoTimestampWithTimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "expiry",
                table: "pass_reset_otp",
                type: "timestamp(6) without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "expiry",
                table: "pass_reset_otp",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) without time zone");
        }
    }
}
