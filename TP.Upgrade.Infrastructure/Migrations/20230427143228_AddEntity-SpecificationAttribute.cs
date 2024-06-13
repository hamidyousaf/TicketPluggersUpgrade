using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitySpecificationAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "TicketTypeId",
                table: "Products",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SpecificationAttributes",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationAttributeOptions",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    SpecificationAttributeId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationAttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecificationAttributeOptions_SpecificationAttributes_SpecificationAttributeId",
                        column: x => x.SpecificationAttributeId,
                        principalTable: "SpecificationAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationAttributeOptions_SpecificationAttributeId",
                table: "SpecificationAttributeOptions",
                column: "SpecificationAttributeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationAttributeOptions");

            migrationBuilder.DropTable(
                name: "SpecificationAttributes");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "TicketTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
