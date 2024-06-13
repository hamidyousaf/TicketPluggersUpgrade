using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPosterActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPosterActive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPosterActive",
                table: "Categories");
        }
    }
}
