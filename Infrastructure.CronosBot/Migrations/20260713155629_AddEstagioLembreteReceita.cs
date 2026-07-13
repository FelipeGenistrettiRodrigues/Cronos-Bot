using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.CronosBot.Migrations
{
    /// <inheritdoc />
    public partial class AddEstagioLembreteReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstagioLembreteReceita",
                schema: "cronosbot",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstagioLembreteReceita",
                schema: "cronosbot",
                table: "Sessions");
        }
    }
}
