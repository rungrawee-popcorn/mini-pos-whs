using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniPOSWHS.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreatedDate", "Price", "ProductCode", "ProductName", "StockQty" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 2, 23, 6, 4, 218, DateTimeKind.Local).AddTicks(6031), 15m, "P001", "Coke", 100 },
                    { 2, new DateTime(2026, 6, 2, 23, 6, 4, 218, DateTimeKind.Local).AddTicks(6033), 10m, "P002", "Water", 100 },
                    { 3, new DateTime(2026, 6, 2, 23, 6, 4, 218, DateTimeKind.Local).AddTicks(6035), 20m, "P003", "Snack", 100 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2026, 6, 2, 23, 6, 4, 218, DateTimeKind.Local).AddTicks(5400), "$2a$11$PkbXzCZrXw04jted5ae.1.e/Nj0JjKIOYhUDfnPywL134LHpRol0u", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
