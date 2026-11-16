using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wt_lab6_6minimalapi_liseychikov.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Category", "CreatedAt", "IsAvaliable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Базы данных", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Проектирование базы данных", 2500 },
                    { 2, "Backend", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), true, "Настройка эндпоинтов API", 1800 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
