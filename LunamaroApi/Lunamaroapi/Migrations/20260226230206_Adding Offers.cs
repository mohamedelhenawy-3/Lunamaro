using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class AddingOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DiscountTiers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDeals_ProductId",
                table: "WeeklyDeals",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AddOnRewards_FreeProductId",
                table: "AddOnRewards",
                column: "FreeProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddOnRewards_Items_FreeProductId",
                table: "AddOnRewards",
                column: "FreeProductId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyDeals_Items_ProductId",
                table: "WeeklyDeals",
                column: "ProductId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddOnRewards_Items_FreeProductId",
                table: "AddOnRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyDeals_Items_ProductId",
                table: "WeeklyDeals");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyDeals_ProductId",
                table: "WeeklyDeals");

            migrationBuilder.DropIndex(
                name: "IX_AddOnRewards_FreeProductId",
                table: "AddOnRewards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DiscountTiers");
        }
    }
}
