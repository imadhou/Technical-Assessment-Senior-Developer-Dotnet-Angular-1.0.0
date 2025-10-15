using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("11111111-aaaa-1111-aaaa-111111111111"),
                column: "PasswordHash",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("22222222-bbbb-2222-bbbb-222222222222"),
                column: "PasswordHash",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "PasswordHash",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "PasswordHash",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                column: "PasswordHash",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");
        }
    }
}
