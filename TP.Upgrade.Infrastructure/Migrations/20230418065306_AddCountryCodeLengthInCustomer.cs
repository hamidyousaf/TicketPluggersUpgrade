using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryCodeLengthInCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Customers",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "44",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "44");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "44",
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldDefaultValue: "44");
        }
    }
}
