using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efreshli.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditWebsiteInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "WebsiteInfos",
                newName: "Office");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "WebsiteInfos",
                newName: "LogoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Office",
                table: "WebsiteInfos",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "WebsiteInfos",
                newName: "NameAr");
        }
    }
}
