using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using TP.Upgrade.Infrastructure.Helpers;

#nullable disable

namespace TP.Upgrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureMigrateEventInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrateProcedure.Add(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrateProcedure.Remove(migrationBuilder);
        }
    }
}
