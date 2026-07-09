using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAHome.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyModuleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Properties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "Properties",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "DeletedAtUtc", "Locality" },
                values: new object[] { null, "SoMa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Locality",
                table: "Properties");
        }
    }
}
