using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Code", "IsActive", "IsSystem", "Name", "Portal", "ScopeType" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000004"), "public-user", true, true, "Public User", 2, 3 });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000004") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));
        }
    }
}
