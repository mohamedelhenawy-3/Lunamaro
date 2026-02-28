using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModeluserorderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "paymentType",
                table: "UserOrderHeaders",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "UserOrderHeaders",
                newName: "TotalDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "SubtotalAmount",
                table: "UserOrderHeaders",
                newName: "TierDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "UserOrderHeaders",
                newName: "OriginalTotalAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalTotalAmount",
                table: "UserOrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OfferDiscountAmount",
                table: "UserOrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemDiscountAmount",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemFinalTotal",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemOriginalTotal",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalTotalAmount",
                table: "UserOrderHeaders");

            migrationBuilder.DropColumn(
                name: "OfferDiscountAmount",
                table: "UserOrderHeaders");

            migrationBuilder.DropColumn(
                name: "ItemDiscountAmount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ItemFinalTotal",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ItemOriginalTotal",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "UserOrderHeaders",
                newName: "paymentType");

            migrationBuilder.RenameColumn(
                name: "TotalDiscountAmount",
                table: "UserOrderHeaders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "TierDiscountAmount",
                table: "UserOrderHeaders",
                newName: "SubtotalAmount");

            migrationBuilder.RenameColumn(
                name: "OriginalTotalAmount",
                table: "UserOrderHeaders",
                newName: "DiscountAmount");
        }
    }
}
