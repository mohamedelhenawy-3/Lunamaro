using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class UPDAINGUSERHEADER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "UserOrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "UserOrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalAmount",
                table: "UserOrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeItem",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "UserOrderHeaders");

            migrationBuilder.DropColumn(
                name: "SubtotalAmount",
                table: "UserOrderHeaders");

            migrationBuilder.DropColumn(
                name: "IsFreeItem",
                table: "OrderItems");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "UserOrderHeaders",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
