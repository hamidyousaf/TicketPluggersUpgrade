using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitySearchKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFavourites_Venues_VenueId",
                table: "CustomerFavourites");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_ShippingAddressId",
                table: "Customers");

            migrationBuilder.CreateTable(
                name: "SearchKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchKeys", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFavourites_Venues_VenueId",
                table: "CustomerFavourites",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Addresses_ShippingAddressId",
                table: "Customers",
                column: "ShippingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFavourites_Venues_VenueId",
                table: "CustomerFavourites");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_ShippingAddressId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "SearchKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFavourites_Venues_VenueId",
                table: "CustomerFavourites",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Addresses_ShippingAddressId",
                table: "Customers",
                column: "ShippingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
