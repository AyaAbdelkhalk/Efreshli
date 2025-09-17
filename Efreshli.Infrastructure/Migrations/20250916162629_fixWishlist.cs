using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efreshli.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_ProductItems_ProductItemId",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "ProductItemId",
                table: "WishlistItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_ProductItemId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "WishlistItems",
                newName: "ProductItemId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_ProductItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_ProductItems_ProductItemId",
                table: "WishlistItems",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
