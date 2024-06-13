using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReportProblems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportProblems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    IsCustomerSend = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChatType = table.Column<byte>(type: "tinyint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProblems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportProblems_CustomerOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportProblems_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportProblems_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportProblems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportProblems_CustomerId",
                table: "ReportProblems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProblems_EventId",
                table: "ReportProblems",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProblems_OrderId",
                table: "ReportProblems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProblems_ProductId",
                table: "ReportProblems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportProblems");
        }
    }
}
