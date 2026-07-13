using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.CronosBot.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCamposControleLembrete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentAt",
                schema: "cronosbot",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "hasPrescription",
                schema: "cronosbot",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSentAt",
                schema: "cronosbot",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "hasPrescription",
                schema: "cronosbot",
                table: "Sessions");
        }
    }
}
