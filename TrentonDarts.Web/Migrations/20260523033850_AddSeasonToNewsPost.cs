using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonToNewsPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "winter_season_id",
                table: "news_posts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_news_posts_winter_season_id",
                table: "news_posts",
                column: "winter_season_id");

            migrationBuilder.AddForeignKey(
                name: "FK_news_posts_winter_seasons_winter_season_id",
                table: "news_posts",
                column: "winter_season_id",
                principalTable: "winter_seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_news_posts_winter_seasons_winter_season_id",
                table: "news_posts");

            migrationBuilder.DropIndex(
                name: "IX_news_posts_winter_season_id",
                table: "news_posts");

            migrationBuilder.DropColumn(
                name: "winter_season_id",
                table: "news_posts");
        }
    }
}
