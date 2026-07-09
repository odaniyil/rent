using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAHome.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseModuleRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SecurityDepositReceived",
                table: "TenantLeases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubleaseAllowed",
                table: "OwnerLeases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "OwnerLeases",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "SubleaseAllowed",
                value: true);

            migrationBuilder.UpdateData(
                table: "TenantLeases",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                column: "SecurityDepositReceived",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityDepositReceived",
                table: "TenantLeases");

            migrationBuilder.DropColumn(
                name: "SubleaseAllowed",
                table: "OwnerLeases");
        }
    }
}
