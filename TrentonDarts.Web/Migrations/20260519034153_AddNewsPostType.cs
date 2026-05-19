using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsPostType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "post_type",
                table: "news_posts",
                type: "text",
                nullable: false,
                defaultValue: "News");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "post_type",
                table: "news_posts");
        }
    }
}
