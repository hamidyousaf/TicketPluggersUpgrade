using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyCodeInEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Events",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Events");
        }
    }
}
