using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "UserOrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderStatus",
                table: "UserOrderHeaders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
