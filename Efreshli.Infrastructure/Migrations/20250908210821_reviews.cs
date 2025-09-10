using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efreshli.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class reviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ReviewId",
                table: "Images",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Reviews_ReviewId",
                table: "Images",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Reviews_ReviewId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ReviewId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Images");
        }
    }
}
