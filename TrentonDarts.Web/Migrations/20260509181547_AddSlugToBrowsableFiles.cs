using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToBrowsableFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "browsable_files",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_browsable_files_slug",
                table: "browsable_files",
                column: "slug",
                unique: true,
                filter: "\"slug\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_browsable_files_slug",
                table: "browsable_files");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "browsable_files");
        }
    }
}
