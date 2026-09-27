using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false),
                    CaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskCode = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TargetRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetCenterId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetUserRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClaimedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClaimedByUserRoleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CaseId",
                table: "Assignments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TargetType_TargetRoleId_TargetCenterId_Status",
                table: "Assignments",
                columns: new[] { "TargetType", "TargetRoleId", "TargetCenterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TargetUserId_Status",
                table: "Assignments",
                columns: new[] { "TargetUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TargetUserRoleId_Status",
                table: "Assignments",
                columns: new[] { "TargetUserRoleId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");
        }
    }
}
