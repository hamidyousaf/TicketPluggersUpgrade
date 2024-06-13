using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProductv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_VendorId",
                table: "Products");

            migrationBuilder.AlterColumn<long>(
                name: "VendorId",
                table: "Products",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Customers_VendorId",
                table: "Products",
                column: "VendorId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Customers_VendorId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_VendorId",
                table: "Products",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
