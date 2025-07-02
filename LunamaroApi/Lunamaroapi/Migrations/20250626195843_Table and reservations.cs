using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class Tableandreservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PostalCode",
                table: "UserOrderHeaders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    ReservationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Capacity", "Location" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 4, 1 },
                    { 3, 6, 1 },
                    { 4, 4, 1 },
                    { 5, 2, 1 },
                    { 6, 6, 1 },
                    { 7, 2, 1 },
                    { 8, 4, 1 },
                    { 9, 6, 1 },
                    { 10, 4, 1 },
                    { 11, 2, 1 },
                    { 12, 6, 1 },
                    { 13, 2, 1 },
                    { 14, 4, 1 },
                    { 15, 6, 1 },
                    { 16, 4, 2 },
                    { 17, 2, 2 },
                    { 18, 6, 2 },
                    { 19, 2, 2 },
                    { 20, 4, 2 },
                    { 21, 6, 2 },
                    { 22, 4, 2 },
                    { 23, 2, 2 },
                    { 24, 6, 2 },
                    { 25, 2, 2 },
                    { 26, 4, 2 },
                    { 27, 6, 2 },
                    { 28, 4, 2 },
                    { 29, 2, 2 },
                    { 30, 6, 2 },
                    { 31, 2, 2 },
                    { 32, 4, 2 },
                    { 33, 6, 2 },
                    { 34, 4, 2 },
                    { 35, 2, 2 },
                    { 36, 6, 3 },
                    { 37, 2, 3 },
                    { 38, 4, 3 },
                    { 39, 6, 3 },
                    { 40, 4, 3 },
                    { 41, 2, 3 },
                    { 42, 6, 3 },
                    { 43, 2, 3 },
                    { 44, 4, 3 },
                    { 45, 6, 3 },
                    { 46, 4, 3 },
                    { 47, 2, 3 },
                    { 48, 6, 3 },
                    { 49, 2, 3 },
                    { 50, 4, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "UserOrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
