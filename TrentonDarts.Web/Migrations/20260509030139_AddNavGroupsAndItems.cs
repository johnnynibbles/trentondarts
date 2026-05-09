using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNavGroupsAndItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nav_groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nav_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nav_group_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NavGroupId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    UrlTemplate = table.Column<string>(type: "text", nullable: true),
                    BrowsableFileId = table.Column<int>(type: "integer", nullable: true),
                    IsHeader = table.Column<bool>(type: "boolean", nullable: false),
                    IsSeparator = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nav_group_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nav_group_items_browsable_files_BrowsableFileId",
                        column: x => x.BrowsableFileId,
                        principalTable: "browsable_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_nav_group_items_nav_groups_NavGroupId",
                        column: x => x.NavGroupId,
                        principalTable: "nav_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nav_group_items_BrowsableFileId",
                table: "nav_group_items",
                column: "BrowsableFileId");

            migrationBuilder.CreateIndex(
                name: "IX_nav_group_items_NavGroupId",
                table: "nav_group_items",
                column: "NavGroupId");

            // Seed default nav groups
            migrationBuilder.Sql(@"
INSERT INTO nav_groups (""Id"", ""Title"", ""SortOrder"", created_at, updated_at)
OVERRIDING SYSTEM VALUE VALUES
  (1, 'Current', 10, NOW(), NOW()),
  (2, 'Activities and Events', 20, NOW(), NOW()),
  (3, 'League', 30, NOW(), NOW()),
  (4, 'Other', 40, NOW(), NOW());
SELECT setval(pg_get_serial_sequence('nav_groups', 'Id'), (SELECT MAX(""Id"") FROM nav_groups));
");

            // Seed default nav items  (ItemType: 0=InternalLink, 1=ExternalUrl, 2=Document)
            migrationBuilder.Sql(@"
INSERT INTO nav_group_items (""Id"", ""NavGroupId"", ""Title"", ""ItemType"", ""UrlTemplate"", ""IsHeader"", ""IsSeparator"", ""SortOrder"", created_at, updated_at)
OVERRIDING SYSTEM VALUE VALUES
  (1,  1, 'Weekly Standings',             0, '/season/{currentSeasonId}',           false, false, 10, NOW(), NOW()),
  (2,  1, 'Full Schedule',                0, '/season/{currentSeasonId}/schedule',   false, false, 20, NOW(), NOW()),
  (3,  1, 'Stats',                        0, '/season/{currentSeasonId}/stats',      false, false, 30, NOW(), NOW()),
  (4,  1, 'Leaderboards',                 0, '/season/{currentSeasonId}/leaderboard',false, false, 40, NOW(), NOW()),
  (5,  1, 'Awards',                       0, '/season/{currentSeasonId}/awards',     false, false, 50, NOW(), NOW()),
  (6,  1, 'Teams',                        0, '/teams',                               false, false, 60, NOW(), NOW()),
  (7,  1, 'GTDL on DartConnect',          1, 'https://tv.dartconnect.com/leaguemenu/gtdl',  false, false, 70, NOW(), NOW()),
  (8,  1, 'GTDL Singles on DartConnect',  1, 'https://tv.dartconnect.com/leaguemenu/gtdls', false, false, 80, NOW(), NOW()),
  (9,  2, 'GTDL Player Results at Events',0, '/event/results',                       false, false, 10, NOW(), NOW()),
  (10, 2, 'Darts for Dreams Info',        0, '/old/charity',                         false, false, 20, NOW(), NOW()),
  (11, 3, 'Where we Play',               0, '/teams',                               false, false, 10, NOW(), NOW()),
  (12, 3, 'Sponsors and Partners',        0, '/sponsor',                             false, false, 20, NOW(), NOW());
SELECT setval(pg_get_serial_sequence('nav_group_items', 'Id'), (SELECT MAX(""Id"") FROM nav_group_items));
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nav_group_items");

            migrationBuilder.DropTable(
                name: "nav_groups");
        }
    }
}
