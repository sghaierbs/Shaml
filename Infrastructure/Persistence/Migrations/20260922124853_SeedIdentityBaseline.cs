using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "center.view", "View Shaml centers" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "center.manage", "Manage Shaml centers" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "case.view", "View cases" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "case.assign", "Assign cases" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "session.view", "View sessions" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "session.schedule", "Schedule sessions" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "user.view", "View users" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "user.assign-role", "Assign roles to users" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Code", "IsActive", "IsSystem", "Name", "Portal", "ScopeType" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "center-director", true, true, "Center Director", 1, 2 },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "specialist", true, true, "Specialist", 1, 2 },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "operations-supervisor", true, true, "Operations Supervisor", 1, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));
        }
    }
}
