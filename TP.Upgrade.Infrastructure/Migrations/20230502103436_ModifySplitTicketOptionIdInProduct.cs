using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifySplitTicketOptionIdInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketSpliting",
                table: "Products");

            migrationBuilder.AddColumn<byte>(
                name: "SplitTicketOptionId",
                table: "Products",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SplitTicketOptionId",
                table: "Products",
                column: "SplitTicketOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SplitTicketOptions_SplitTicketOptionId",
                table: "Products",
                column: "SplitTicketOptionId",
                principalTable: "SplitTicketOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SplitTicketOptions_SplitTicketOptionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SplitTicketOptionId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SplitTicketOptionId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "TicketSpliting",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
