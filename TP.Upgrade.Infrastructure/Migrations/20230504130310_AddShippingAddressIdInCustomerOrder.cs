using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingAddressIdInCustomerOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_ShippingAddressId",
                table: "CustomerOrders",
                column: "ShippingAddressId",
                unique: true,
                filter: "[ShippingAddressId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_Addresses_ShippingAddressId",
                table: "CustomerOrders",
                column: "ShippingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Addresses_ShippingAddressId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_ShippingAddressId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "CustomerOrders");
        }
    }
}
