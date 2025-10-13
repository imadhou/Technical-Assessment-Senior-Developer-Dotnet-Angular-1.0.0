using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace N2J.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExpenseCategories_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ExpenseCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ReceiptUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Expenses_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "ID", "Address", "CreatedAt", "Currency", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "1 Rue de la Paix, 75002 Paris, France", new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "EUR", "N2JSoft", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "123 Tech Avenue, San Francisco, CA, USA", new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "USD", "Tech Solutions", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Administrator with full access", "Admin" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Manager with limited access", "Manager" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Employee with basic access", "Employee" }
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "ID", "CompanyId", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Expenses related to travel", "Travel" },
                    { new Guid("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Expenses for meals and entertainment", "Meals" },
                    { new Guid("cccccccc-3333-cccc-3333-cccccccc3333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Expenses for office supplies and equipment", "Office Supplies" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "CompanyId", "CreatedAt", "Email", "FirstName", "LastName", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-1111-aaaa-111111111111"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "lando.norris@techsol.us", "Lando", "Norris", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null },
                    { new Guid("22222222-bbbb-2222-bbbb-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "george.russell@techsol.us", "George", "Russell", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "charles.leclerc@n2j.fr", "Charles", "Leclerc", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "max.verstappen@n2j.fr", "Max", "Verstappen", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), "oscar.piastri@n2j.fr", "Oscar", "Piastri", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "ID", "Amount", "CreatedAt", "Date", "Description", "ExpenseCategoryId", "ReceiptUrl", "Status", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("aaaa1111-aaaa-1111-aaaa-aaaaaaaaaaaa"), 500m, new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), "Flight ticket to attend the tech conference", new Guid("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"), null, "Pending", "Flight to Conference", null, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd") },
                    { new Guid("bbbb2222-bbbb-2222-bbbb-bbbbbbbbbbbb"), 150m, new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 7, 12, 0, 0, 0, DateTimeKind.Utc), "Lunch with the team to discuss project updates", new Guid("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"), null, "Pending", "Team Lunch", null, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee") },
                    { new Guid("cccc3333-cccc-3333-cccc-cccccccccccc"), 300m, new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Ergonomic office chair for better comfort", new Guid("cccccccc-3333-cccc-3333-cccccccc3333"), null, "Pending", "Office Chair", null, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_CompanyId",
                table: "ExpenseCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
