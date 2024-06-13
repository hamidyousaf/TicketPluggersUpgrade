using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDocumentv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDocuments_CustomerOrders_OrderId",
                table: "ProductDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ProductDocuments_OrderId",
                table: "ProductDocuments");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "ProductDocuments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDocuments_OrderId",
                table: "ProductDocuments",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDocuments_CustomerOrders_OrderId",
                table: "ProductDocuments",
                column: "OrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id");
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

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "ProductDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
