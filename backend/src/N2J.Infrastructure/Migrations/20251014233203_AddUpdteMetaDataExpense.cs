using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdteMetaDataExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("aaaa1111-aaaa-1111-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("bbbb2222-bbbb-2222-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("cccc3333-cccc-3333-cccc-cccccccccccc"),
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Expenses");
        }
    }
}
