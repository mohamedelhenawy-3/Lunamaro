using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class UserCartt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCart_AspNetUsers_UserId",
                table: "UserCart");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCart_Items_ItemId",
                table: "UserCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCart",
                table: "UserCart");

            migrationBuilder.RenameTable(
                name: "UserCart",
                newName: "UserCarts");

            migrationBuilder.RenameIndex(
                name: "IX_UserCart_UserId",
                table: "UserCarts",
                newName: "IX_UserCarts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCart_ItemId",
                table: "UserCarts",
                newName: "IX_UserCarts_ItemId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserCarts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCarts",
                table: "UserCarts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCarts_ApplicationUserId",
                table: "UserCarts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCarts_AspNetUsers_ApplicationUserId",
                table: "UserCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCarts_AspNetUsers_UserId",
                table: "UserCarts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCarts_Items_ItemId",
                table: "UserCarts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCarts_AspNetUsers_ApplicationUserId",
                table: "UserCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCarts_AspNetUsers_UserId",
                table: "UserCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCarts_Items_ItemId",
                table: "UserCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCarts",
                table: "UserCarts");

            migrationBuilder.DropIndex(
                name: "IX_UserCarts_ApplicationUserId",
                table: "UserCarts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserCarts");

            migrationBuilder.RenameTable(
                name: "UserCarts",
                newName: "UserCart");

            migrationBuilder.RenameIndex(
                name: "IX_UserCarts_UserId",
                table: "UserCart",
                newName: "IX_UserCart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCarts_ItemId",
                table: "UserCart",
                newName: "IX_UserCart_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCart",
                table: "UserCart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCart_AspNetUsers_UserId",
                table: "UserCart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCart_Items_ItemId",
                table: "UserCart",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
