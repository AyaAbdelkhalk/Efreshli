using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efreshli.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class prds2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductItemId",
                table: "Colors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ProductItemId",
                table: "Colors",
                column: "ProductItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colors_ProductItems_ProductItemId",
                table: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Colors_ProductItemId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "ProductItemId",
                table: "Colors");
        }
    }
}
