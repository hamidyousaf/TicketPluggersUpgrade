using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDocumentv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "ProductDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProductDocuments_OrderId",
                table: "ProductDocuments",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDocuments_CustomerOrders_OrderId",
                table: "ProductDocuments",
                column: "OrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDocuments_CustomerOrders_OrderId",
                table: "ProductDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ProductDocuments_OrderId",
                table: "ProductDocuments");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductDocuments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductDocuments");
        }
    }
}
