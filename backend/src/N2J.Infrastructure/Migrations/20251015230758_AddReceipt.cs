using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptFileName",
                table: "Expenses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReceiptFileSize",
                table: "Expenses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiptUploadedAt",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptFileName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ReceiptFileSize",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ReceiptUploadedAt",
                table: "Expenses");
        }
    }
}
