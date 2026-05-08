using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gameNumber",
                table: "winter_stats_player_games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gameNumber",
                table: "winter_stats_player_games");
        }
    }
}
