using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "board_members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    userId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: true),
                    startingSeason = table.Column<string>(type: "text", nullable: true),
                    endingSeason = table.Column<string>(type: "text", nullable: true),
                    startSeasonId = table.Column<int>(type: "integer", nullable: true),
                    endSeasonId = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_board_members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "browsable_files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(type: "text", nullable: true),
                    fileName = table.Column<string>(type: "text", nullable: true),
                    relativePath = table.Column<string>(type: "text", nullable: true),
                    mimeType = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_browsable_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dart_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    eventContact = table.Column<string>(type: "text", nullable: true),
                    eventContact2 = table.Column<string>(type: "text", nullable: true),
                    eventTypeId = table.Column<int>(type: "integer", nullable: true),
                    eventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    eventEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dartType = table.Column<string>(type: "text", nullable: true),
                    imageFileId = table.Column<int>(type: "integer", nullable: true),
                    posterFileId = table.Column<int>(type: "integer", nullable: true),
                    posterFile = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    facebookUrl = table.Column<string>(type: "text", nullable: true),
                    hostName = table.Column<string>(type: "text", nullable: true),
                    hostUrl = table.Column<string>(type: "text", nullable: true),
                    hostPhone = table.Column<string>(type: "text", nullable: true),
                    locationName = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Zip = table.Column<string>(type: "text", nullable: true),
                    registrationStartTime = table.Column<string>(type: "text", nullable: true),
                    registrationEndTime = table.Column<string>(type: "text", nullable: true),
                    dartStart = table.Column<string>(type: "text", nullable: true),
                    mapUrl = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    isTitleEvent = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dart_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "match_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "page_parts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Html = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_page_parts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    userId = table.Column<string>(type: "text", nullable: true),
                    firstName = table.Column<string>(type: "text", nullable: false),
                    lastName = table.Column<string>(type: "text", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    homePhone = table.Column<string>(type: "text", nullable: true),
                    cellPhone = table.Column<string>(type: "text", nullable: true),
                    shirtSize = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Zip = table.Column<string>(type: "text", nullable: true),
                    acceptText = table.Column<bool>(type: "boolean", nullable: false),
                    acceptEmail = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    contactName = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Zip = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    facebookUrl = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    mapUrl = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_player_payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    playerId = table.Column<int>(type: "integer", nullable: false),
                    paymentStatus = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_player_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_team_payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    paymentStatus = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_team_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    startYear = table.Column<int>(type: "integer", nullable: false),
                    endYear = table.Column<int>(type: "integer", nullable: false),
                    seasonType = table.Column<string>(type: "text", nullable: true),
                    isCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    defaultMatchTypeId = table.Column<int>(type: "integer", nullable: true),
                    isUsingMatchPoints = table.Column<bool>(type: "boolean", nullable: false),
                    winPoints = table.Column<int>(type: "integer", nullable: false),
                    halfPoints = table.Column<int>(type: "integer", nullable: false),
                    minPointForHalfPoints = table.Column<int>(type: "integer", nullable: false),
                    accumulatePointsForAllParts = table.Column<bool>(type: "boolean", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_stats_awards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    seasonPart = table.Column<string>(type: "text", nullable: true),
                    matchId = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<string>(type: "text", nullable: true),
                    gameId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    teamName = table.Column<string>(type: "text", nullable: true),
                    playerId = table.Column<int>(type: "integer", nullable: false),
                    playerName = table.Column<string>(type: "text", nullable: true),
                    awardId = table.Column<int>(type: "integer", nullable: false),
                    awardType = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_stats_awards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_stats_matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    seasonPart = table.Column<string>(type: "text", nullable: true),
                    matchId = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    teamName = table.Column<string>(type: "text", nullable: true),
                    pointsWon = table.Column<int>(type: "integer", nullable: false),
                    pointsLost = table.Column<int>(type: "integer", nullable: false),
                    matchPoints = table.Column<int>(type: "integer", nullable: false),
                    homeMatch = table.Column<bool>(type: "boolean", nullable: false),
                    hasScorecard = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_stats_matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_stats_player_games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    seasonPart = table.Column<string>(type: "text", nullable: true),
                    matchId = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<string>(type: "text", nullable: true),
                    gameId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    teamName = table.Column<string>(type: "text", nullable: true),
                    playerId = table.Column<int>(type: "integer", nullable: false),
                    playerName = table.Column<string>(type: "text", nullable: true),
                    playerPosition = table.Column<int>(type: "integer", nullable: false),
                    gameType = table.Column<string>(type: "text", nullable: true),
                    numberOfPlayers = table.Column<int>(type: "integer", nullable: false),
                    numberOfPoints = table.Column<int>(type: "integer", nullable: false),
                    isWon = table.Column<bool>(type: "boolean", nullable: false),
                    isForfeit = table.Column<bool>(type: "boolean", nullable: false),
                    isHome = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_stats_player_games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "winter_stats_team_games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    seasonPart = table.Column<string>(type: "text", nullable: true),
                    matchId = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<string>(type: "text", nullable: true),
                    gameId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    teamName = table.Column<string>(type: "text", nullable: true),
                    gameType = table.Column<string>(type: "text", nullable: true),
                    numberOfPlayers = table.Column<int>(type: "integer", nullable: false),
                    numberOfPoints = table.Column<int>(type: "integer", nullable: false),
                    isWon = table.Column<bool>(type: "boolean", nullable: false),
                    isForfeitGame = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_stats_team_games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_type_game_rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    matchTypeId = table.Column<int>(type: "integer", nullable: false),
                    gameType = table.Column<string>(type: "text", nullable: false),
                    doubleIn = table.Column<bool>(type: "boolean", nullable: false),
                    doubleOut = table.Column<bool>(type: "boolean", nullable: false),
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    bestOfNumberOfLegs = table.Column<int>(type: "integer", nullable: true),
                    numberOfLegs = table.Column<int>(type: "integer", nullable: false),
                    whoStarts = table.Column<string>(type: "text", nullable: true),
                    numberOfPlayers = table.Column<int>(type: "integer", nullable: false),
                    gamePointValue = table.Column<int>(type: "integer", nullable: false),
                    legPointValue = table.Column<int>(type: "integer", nullable: true),
                    forfeitIfNoPlayers = table.Column<bool>(type: "boolean", nullable: false),
                    groupName = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_type_game_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_match_type_game_rules_match_types_matchTypeId",
                        column: x => x.matchTypeId,
                        principalTable: "match_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dart_event_results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eventId = table.Column<int>(type: "integer", nullable: false),
                    specificEventName = table.Column<string>(type: "text", nullable: true),
                    playerId = table.Column<int>(type: "integer", nullable: true),
                    Finished = table.Column<string>(type: "text", nullable: true),
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dart_event_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dart_event_results_dart_events_eventId",
                        column: x => x.eventId,
                        principalTable: "dart_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dart_event_results_players_playerId",
                        column: x => x.playerId,
                        principalTable: "players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    sponsorId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_sponsors_sponsorId",
                        column: x => x.sponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_weeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    weekType = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_weeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_season_weeks_winter_seasons_seasonId",
                        column: x => x.seasonId,
                        principalTable: "winter_seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    teamId = table.Column<int>(type: "integer", nullable: false),
                    preSeasonDiv = table.Column<string>(type: "text", nullable: true),
                    regularSeasonDiv = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_season_teams_teams_teamId",
                        column: x => x.teamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_teams_winter_seasons_seasonId",
                        column: x => x.seasonId,
                        principalTable: "winter_seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    weekId = table.Column<int>(type: "integer", nullable: false),
                    matchTypeId = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<string>(type: "text", nullable: true),
                    homeTeamId = table.Column<int>(type: "integer", nullable: false),
                    awayTeamId = table.Column<int>(type: "integer", nullable: false),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_season_matches_match_types_matchTypeId",
                        column: x => x.matchTypeId,
                        principalTable: "match_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_matches_teams_awayTeamId",
                        column: x => x.awayTeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_matches_teams_homeTeamId",
                        column: x => x.homeTeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_matches_winter_season_weeks_weekId",
                        column: x => x.weekId,
                        principalTable: "winter_season_weeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_matches_winter_seasons_seasonId",
                        column: x => x.seasonId,
                        principalTable: "winter_seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_season_team_players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leagueId = table.Column<int>(type: "integer", nullable: false),
                    seasonId = table.Column<int>(type: "integer", nullable: false),
                    seasonTeamId = table.Column<int>(type: "integer", nullable: false),
                    playerId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_season_team_players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_season_team_players_players_playerId",
                        column: x => x.playerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_team_players_winter_season_teams_seasonTeamId",
                        column: x => x.seasonTeamId,
                        principalTable: "winter_season_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_season_team_players_winter_seasons_seasonId",
                        column: x => x.seasonId,
                        principalTable: "winter_seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_match_results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    hasScorecard = table.Column<bool>(type: "boolean", nullable: false),
                    awayScoreOverride = table.Column<int>(type: "integer", nullable: true),
                    homeScoreOverride = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_match_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_match_results_winter_season_matches_Id",
                        column: x => x.Id,
                        principalTable: "winter_season_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_game_results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    matchId = table.Column<int>(type: "integer", nullable: false),
                    homePlayers = table.Column<string>(type: "text", nullable: true),
                    awayPlayers = table.Column<string>(type: "text", nullable: true),
                    Legs = table.Column<string>(type: "text", nullable: true),
                    forfeitedBy = table.Column<string>(type: "text", nullable: true),
                    gameRuleId = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_game_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_game_results_match_type_game_rules_gameRuleId",
                        column: x => x.gameRuleId,
                        principalTable: "match_type_game_rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_game_results_winter_match_results_matchId",
                        column: x => x.matchId,
                        principalTable: "winter_match_results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winter_game_awards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gameId = table.Column<int>(type: "integer", nullable: false),
                    playerId = table.Column<int>(type: "integer", nullable: false),
                    awardType = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winter_game_awards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_winter_game_awards_players_playerId",
                        column: x => x.playerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_winter_game_awards_winter_game_results_gameId",
                        column: x => x.gameId,
                        principalTable: "winter_game_results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_dart_event_results_eventId",
                table: "dart_event_results",
                column: "eventId");

            migrationBuilder.CreateIndex(
                name: "IX_dart_event_results_playerId",
                table: "dart_event_results",
                column: "playerId");

            migrationBuilder.CreateIndex(
                name: "IX_match_type_game_rules_matchTypeId",
                table: "match_type_game_rules",
                column: "matchTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_sponsorId",
                table: "teams",
                column: "sponsorId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_winter_game_awards_gameId",
                table: "winter_game_awards",
                column: "gameId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_game_awards_playerId",
                table: "winter_game_awards",
                column: "playerId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_game_results_gameRuleId",
                table: "winter_game_results",
                column: "gameRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_game_results_matchId",
                table: "winter_game_results",
                column: "matchId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_matches_awayTeamId",
                table: "winter_season_matches",
                column: "awayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_matches_homeTeamId",
                table: "winter_season_matches",
                column: "homeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_matches_matchTypeId",
                table: "winter_season_matches",
                column: "matchTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_matches_seasonId",
                table: "winter_season_matches",
                column: "seasonId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_matches_weekId",
                table: "winter_season_matches",
                column: "weekId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_team_players_playerId",
                table: "winter_season_team_players",
                column: "playerId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_team_players_seasonId",
                table: "winter_season_team_players",
                column: "seasonId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_team_players_seasonTeamId",
                table: "winter_season_team_players",
                column: "seasonTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_teams_seasonId",
                table: "winter_season_teams",
                column: "seasonId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_teams_teamId",
                table: "winter_season_teams",
                column: "teamId");

            migrationBuilder.CreateIndex(
                name: "IX_winter_season_weeks_seasonId",
                table: "winter_season_weeks",
                column: "seasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "board_members");

            migrationBuilder.DropTable(
                name: "browsable_files");

            migrationBuilder.DropTable(
                name: "dart_event_results");

            migrationBuilder.DropTable(
                name: "page_parts");

            migrationBuilder.DropTable(
                name: "winter_game_awards");

            migrationBuilder.DropTable(
                name: "winter_season_player_payments");

            migrationBuilder.DropTable(
                name: "winter_season_team_payments");

            migrationBuilder.DropTable(
                name: "winter_season_team_players");

            migrationBuilder.DropTable(
                name: "winter_stats_awards");

            migrationBuilder.DropTable(
                name: "winter_stats_matches");

            migrationBuilder.DropTable(
                name: "winter_stats_player_games");

            migrationBuilder.DropTable(
                name: "winter_stats_team_games");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "dart_events");

            migrationBuilder.DropTable(
                name: "winter_game_results");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "winter_season_teams");

            migrationBuilder.DropTable(
                name: "match_type_game_rules");

            migrationBuilder.DropTable(
                name: "winter_match_results");

            migrationBuilder.DropTable(
                name: "winter_season_matches");

            migrationBuilder.DropTable(
                name: "match_types");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "winter_season_weeks");

            migrationBuilder.DropTable(
                name: "sponsors");

            migrationBuilder.DropTable(
                name: "winter_seasons");
        }
    }
}
