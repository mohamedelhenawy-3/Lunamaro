using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class AddingAddCartPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_userCartAddOns_UserCartId",
                table: "userCartAddOns",
                newName: "IX_UserCartAddOns_UserCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_UserCartAddOns_UserCartId",
                table: "userCartAddOns",
                newName: "IX_userCartAddOns_UserCartId");
        }
    }
}
