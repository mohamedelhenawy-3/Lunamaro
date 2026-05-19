using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunamaroapi.Migrations
{
    /// <inheritdoc />
    public partial class AddingAddoneupdateusercar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoryRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Priorty = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    RelatedCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoryRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_categoryRelationships_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_categoryRelationships_Categories_RelatedCategoryId",
                        column: x => x.RelatedCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemAddOns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAddOns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAddOns_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RelatedItemId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ItemId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRelationships_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemRelationships_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemRelationships_Items_RelatedItemId",
                        column: x => x.RelatedItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userCartAddOns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCartId = table.Column<int>(type: "int", nullable: false),
                    AddOnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userCartAddOns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userCartAddOns_ItemAddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "ItemAddOns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userCartAddOns_UserCarts_UserCartId",
                        column: x => x.UserCartId,
                        principalTable: "UserCarts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_categoryRelationships_CategoryId",
                table: "categoryRelationships",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_categoryRelationships_RelatedCategoryId",
                table: "categoryRelationships",
                column: "RelatedCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAddOns_ItemId",
                table: "ItemAddOns",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelationships_ItemId",
                table: "ItemRelationships",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelationships_ItemId1",
                table: "ItemRelationships",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelationships_RelatedItemId",
                table: "ItemRelationships",
                column: "RelatedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_userCartAddOns_AddOnId",
                table: "userCartAddOns",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_userCartAddOns_UserCartId",
                table: "userCartAddOns",
                column: "UserCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categoryRelationships");

            migrationBuilder.DropTable(
                name: "ItemRelationships");

            migrationBuilder.DropTable(
                name: "userCartAddOns");

            migrationBuilder.DropTable(
                name: "ItemAddOns");
        }
    }
}
