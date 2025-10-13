using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeConvension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("cccccccc-3333-cccc-3333-cccccccc3333"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("aaaa1111-aaaa-1111-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("bbbb2222-bbbb-2222-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("cccc3333-cccc-3333-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("11111111-aaaa-1111-aaaa-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("22222222-bbbb-2222-bbbb-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ExpenseCategories",
                keyColumn: "ID",
                keyValue: new Guid("cccccccc-3333-cccc-3333-cccccccc3333"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("aaaa1111-aaaa-1111-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("bbbb2222-bbbb-2222-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "ID",
                keyValue: new Guid("cccc3333-cccc-3333-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "Date", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("11111111-aaaa-1111-aaaa-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("22222222-bbbb-2222-bbbb-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });
        }
    }
}
