using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efreshli.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
