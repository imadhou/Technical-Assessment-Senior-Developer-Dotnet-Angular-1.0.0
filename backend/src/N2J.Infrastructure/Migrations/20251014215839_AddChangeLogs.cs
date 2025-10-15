using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ChangesJson = table.Column<string>(type: "jsonb", nullable: true),
                    ChangedBy = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntityState = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLog", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_ChangedAt",
                table: "ChangeLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_ChangedBy",
                table: "ChangeLogs",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_ChangeType",
                table: "ChangeLogs",
                column: "ChangeType");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_EntityId",
                table: "ChangeLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_EntityName",
                table: "ChangeLogs",
                column: "EntityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeLogs");
        }
    }
}
