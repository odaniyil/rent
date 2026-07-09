using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAHome.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AuthUsers",
                columns: new[] { "Id", "CreatedAtUtc", "Email", "PasswordHash", "Role", "Status" },
                values: new object[] { new Guid("80000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "superadmin@rentahome.local", "pbkdf2_sha256$100000$cmVudC1hLWhvbWUtZGVtby1zdXBlcmFkbWluLXNhbHQ=$q9OkGlrDjiENYIp50z1Vfq4GFx7weRi/qX1Bg9ki/nI=", "SuperAdmin", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_Email",
                table: "AuthUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthUsers");
        }
    }
}
