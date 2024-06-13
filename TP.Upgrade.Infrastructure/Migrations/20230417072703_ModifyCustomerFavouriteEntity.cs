using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCustomerFavouriteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFavourites_Customers_CustomerId",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CustomerFavourites");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CustomerFavourites");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "CustomerFavourites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PerformerId",
                table: "CustomerFavourites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "CustomerFavourites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFavourites_Customers_CustomerId",
                table: "CustomerFavourites",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFavourites_Customers_CustomerId",
                table: "CustomerFavourites");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "CustomerFavourites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PerformerId",
                table: "CustomerFavourites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "CustomerFavourites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CustomerFavourites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CustomerFavourites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "CustomerFavourites",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomerFavourites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CustomerFavourites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CustomerFavourites",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFavourites_Customers_CustomerId",
                table: "CustomerFavourites",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
