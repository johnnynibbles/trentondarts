using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using MatchType = TrentonDarts.Web.Data.Entities.MatchType;

namespace TrentonDarts.Web.Data;

public class AppDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();
    public DbSet<MatchType> MatchTypes => Set<MatchType>();
    public DbSet<MatchTypeGameRule> MatchTypeGameRules => Set<MatchTypeGameRule>();
    public DbSet<BrowsableFile> BrowsableFiles => Set<BrowsableFile>();
    public DbSet<WinterSeason> WinterSeasons => Set<WinterSeason>();
    public DbSet<WinterSeasonTeam> WinterSeasonTeams => Set<WinterSeasonTeam>();
    public DbSet<WinterSeasonTeamPlayer> WinterSeasonTeamPlayers => Set<WinterSeasonTeamPlayer>();
    public DbSet<WinterSeasonWeek> WinterSeasonWeeks => Set<WinterSeasonWeek>();
    public DbSet<WinterSeasonMatch> WinterSeasonMatches => Set<WinterSeasonMatch>();
    public DbSet<WinterMatchResult> WinterMatchResults => Set<WinterMatchResult>();
    public DbSet<WinterGameResult> WinterGameResults => Set<WinterGameResult>();
    public DbSet<WinterGameAward> WinterGameAwards => Set<WinterGameAward>();
    public DbSet<WinterSeasonPlayerPayment> WinterSeasonPlayerPayments => Set<WinterSeasonPlayerPayment>();
    public DbSet<WinterSeasonTeamPayment> WinterSeasonTeamPayments => Set<WinterSeasonTeamPayment>();
    public DbSet<WinterStatMatch> WinterStatMatches => Set<WinterStatMatch>();
    public DbSet<WinterStatTeamGame> WinterStatTeamGames => Set<WinterStatTeamGame>();
    public DbSet<WinterStatPlayerGame> WinterStatPlayerGames => Set<WinterStatPlayerGame>();
    public DbSet<WinterStatAward> WinterStatAwards => Set<WinterStatAward>();
    public DbSet<DartEvent> DartEvents => Set<DartEvent>();
    public DbSet<DartEventResult> DartEventResults => Set<DartEventResult>();
    public DbSet<PagePart> PageParts => Set<PagePart>();
    public DbSet<NavGroup> NavGroups => Set<NavGroup>();
    public DbSet<NavGroupItem> NavGroupItems => Set<NavGroupItem>();
    public DbSet<NewsPost> NewsPosts => Set<NewsPost>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<SeasonPart>()
            .HaveConversion<SeasonPartConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Map Identity tables
        builder.Entity<User>().ToTable("users");

        // ── Player ──────────────────────────────────────────────────────────
        builder.Entity<Player>(e =>
        {
            e.ToTable("players");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.UserId).HasColumnName("userId");
            e.Property(p => p.FirstName).HasColumnName("firstName");
            e.Property(p => p.LastName).HasColumnName("lastName");
            e.Property(p => p.Nickname).HasColumnName("nickname");
            e.Property(p => p.HomePhone).HasColumnName("homePhone");
            e.Property(p => p.CellPhone).HasColumnName("cellPhone");
            e.Property(p => p.ShirtSize).HasColumnName("shirtSize");
            e.Property(p => p.Address1).HasColumnName("address1");
            e.Property(p => p.Address2).HasColumnName("address2");
            e.Property(p => p.AcceptText).HasColumnName("acceptText");
            e.Property(p => p.AcceptEmail).HasColumnName("acceptEmail");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.Ignore(p => p.Name);
        });

        // ── Sponsor ──────────────────────────────────────────────────────────
        builder.Entity<Sponsor>(e =>
        {
            e.ToTable("sponsors");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.ContactName).HasColumnName("contactName");
            e.Property(p => p.Address1).HasColumnName("address1");
            e.Property(p => p.Address2).HasColumnName("address2");
            e.Property(p => p.FacebookUrl).HasColumnName("facebookUrl");
            e.Property(p => p.MapUrl).HasColumnName("mapUrl");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasMany(s => s.Teams).WithOne(t => t.Sponsor).HasForeignKey(t => t.SponsorId);
        });

        // ── Team ──────────────────────────────────────────────────────────────
        builder.Entity<Team>(e =>
        {
            e.ToTable("teams");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.SponsorId).HasColumnName("sponsorId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
        });

        // ── BoardMember ───────────────────────────────────────────────────────
        builder.Entity<BoardMember>(e =>
        {
            e.ToTable("board_members");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.UserId).HasColumnName("userId");
            e.Property(p => p.StartingSeason).HasColumnName("startingSeason");
            e.Property(p => p.EndingSeason).HasColumnName("endingSeason");
            e.Property(p => p.StartSeasonId).HasColumnName("startSeasonId");
            e.Property(p => p.EndSeasonId).HasColumnName("endSeasonId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
        });

        // ── MatchType ─────────────────────────────────────────────────────────
        builder.Entity<MatchType>(e =>
        {
            e.ToTable("match_types");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasMany(m => m.GameRules).WithOne(r => r.MatchType).HasForeignKey(r => r.MatchTypeId);
        });

        // ── MatchTypeGameRule ─────────────────────────────────────────────────
        builder.Entity<MatchTypeGameRule>(e =>
        {
            e.ToTable("match_type_game_rules");
            e.Property(p => p.MatchTypeId).HasColumnName("matchTypeId");
            e.Property(p => p.GameType).HasColumnName("gameType");
            e.Property(p => p.DoubleIn).HasColumnName("doubleIn");
            e.Property(p => p.DoubleOut).HasColumnName("doubleOut");
            e.Property(p => p.OrderId).HasColumnName("orderId");
            e.Property(p => p.BestOfNumberOfLegs).HasColumnName("bestOfNumberOfLegs");
            e.Property(p => p.NumberOfLegs).HasColumnName("numberOfLegs");
            e.Property(p => p.WhoStarts).HasColumnName("whoStarts");
            e.Property(p => p.NumberOfPlayers).HasColumnName("numberOfPlayers");
            e.Property(p => p.GamePointValue).HasColumnName("gamePointValue");
            e.Property(p => p.LegPointValue).HasColumnName("legPointValue");
            e.Property(p => p.ForfeitIfNoPlayers).HasColumnName("forfeitIfNoPlayers");
            e.Property(p => p.GroupName).HasColumnName("groupName");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(r => r.MatchType.DeletedAt == null && r.DeletedAt == null);
        });

        // ── BrowsableFile ─────────────────────────────────────────────────────
        builder.Entity<BrowsableFile>(e =>
        {
            e.ToTable("browsable_files");
            e.Property(p => p.Slug).HasColumnName("slug");
            e.Property(p => p.FileName).HasColumnName("fileName");
            e.Property(p => p.RelativePath).HasColumnName("relativePath");
            e.Property(p => p.MimeType).HasColumnName("mimeType");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasIndex(p => p.Slug).IsUnique().HasFilter("\"slug\" IS NOT NULL");
            e.HasQueryFilter(p => p.DeletedAt == null);
        });

        // ── WinterSeason ──────────────────────────────────────────────────────
        builder.Entity<WinterSeason>(e =>
        {
            e.ToTable("winter_seasons");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.StartYear).HasColumnName("startYear");
            e.Property(p => p.EndYear).HasColumnName("endYear");
            e.Property(p => p.SeasonType).HasColumnName("seasonType");
            e.Property(p => p.IsCurrent).HasColumnName("isCurrent");
            e.Property(p => p.DefaultMatchTypeId).HasColumnName("defaultMatchTypeId");
            e.Property(p => p.IsUsingMatchPoints).HasColumnName("isUsingMatchPoints");
            e.Property(p => p.WinPoints).HasColumnName("winPoints");
            e.Property(p => p.HalfPoints).HasColumnName("halfPoints");
            e.Property(p => p.MinPointForHalfPoints).HasColumnName("minPointForHalfPoints");
            e.Property(p => p.AccumulatePointsForAllParts).HasColumnName("accumulatePointsForAllParts");
            e.Property(p => p.PublishedAt).HasColumnName("published_at");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasMany(s => s.SeasonTeams).WithOne(st => st.Season).HasForeignKey(st => st.SeasonId);
            e.HasMany(s => s.Weeks).WithOne(w => w.Season).HasForeignKey(w => w.SeasonId);
        });

        // ── WinterSeasonTeam ──────────────────────────────────────────────────
        builder.Entity<WinterSeasonTeam>(e =>
        {
            e.ToTable("winter_season_teams");
            e.HasQueryFilter(st => st.Season.DeletedAt == null);
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.PreSeasonDiv).HasColumnName("preSeasonDiv");
            e.Property(p => p.RegularSeasonDiv).HasColumnName("regularSeasonDiv");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(st => st.Team).WithMany().HasForeignKey(st => st.TeamId);
            e.HasMany(st => st.TeamPlayers).WithOne(tp => tp.SeasonTeam).HasForeignKey(tp => tp.SeasonTeamId);
        });

        // ── WinterSeasonTeamPlayer ────────────────────────────────────────────
        builder.Entity<WinterSeasonTeamPlayer>(e =>
        {
            e.ToTable("winter_season_team_players");
            e.HasQueryFilter(tp => tp.Player.DeletedAt == null);
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.SeasonTeamId).HasColumnName("seasonTeamId");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(tp => tp.Season).WithMany().HasForeignKey(tp => tp.SeasonId);
            e.HasOne(tp => tp.Player).WithMany().HasForeignKey(tp => tp.PlayerId);
        });

        // ── WinterSeasonWeek ──────────────────────────────────────────────────
        builder.Entity<WinterSeasonWeek>(e =>
        {
            e.ToTable("winter_season_weeks");
            e.HasQueryFilter(w => w.Season.DeletedAt == null);
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.WeekType).HasColumnName("weekType");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasMany(w => w.Matches).WithOne(m => m.Week).HasForeignKey(m => m.WeekId);
        });

        // ── WinterSeasonMatch ─────────────────────────────────────────────────
        builder.Entity<WinterSeasonMatch>(e =>
        {
            e.ToTable("winter_season_matches");
            e.HasQueryFilter(m => m.HomeTeam.DeletedAt == null && m.AwayTeam.DeletedAt == null);
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.WeekId).HasColumnName("weekId");
            e.Property(p => p.MatchTypeId).HasColumnName("matchTypeId");
            e.Property(p => p.HomeTeamId).HasColumnName("homeTeamId");
            e.Property(p => p.AwayTeamId).HasColumnName("awayTeamId");
            e.Property(p => p.LeagueId).HasColumnName("leagueId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(m => m.Season).WithMany().HasForeignKey(m => m.SeasonId);
            e.HasOne(m => m.HomeTeam).WithMany().HasForeignKey(m => m.HomeTeamId);
            e.HasOne(m => m.AwayTeam).WithMany().HasForeignKey(m => m.AwayTeamId);
            e.HasOne(m => m.MatchType).WithMany().HasForeignKey(m => m.MatchTypeId);
            e.HasOne(m => m.MatchResult).WithOne(mr => mr.Match).HasForeignKey<WinterMatchResult>(mr => mr.Id);
        });

        // ── WinterMatchResult ─────────────────────────────────────────────────
        builder.Entity<WinterMatchResult>(e =>
        {
            e.ToTable("winter_match_results");
            e.HasQueryFilter(mr => mr.Match.HomeTeam.DeletedAt == null && mr.Match.AwayTeam.DeletedAt == null);
            e.Property(p => p.HasScorecard).HasColumnName("hasScorecard");
            e.Property(p => p.AwayScoreOverride).HasColumnName("awayScoreOverride");
            e.Property(p => p.HomeScoreOverride).HasColumnName("homeScoreOverride");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            // Id is FK to WinterSeasonMatch — not auto-generated
            e.Property(p => p.Id).ValueGeneratedNever();
            e.HasMany(mr => mr.GameResults).WithOne(gr => gr.MatchResult).HasForeignKey(gr => gr.MatchId);
        });

        // ── WinterGameResult ──────────────────────────────────────────────────
        builder.Entity<WinterGameResult>(e =>
        {
            e.ToTable("winter_game_results");
            e.HasQueryFilter(gr => gr.GameRule.MatchType.DeletedAt == null);
            e.Property(p => p.MatchId).HasColumnName("matchId");
            e.Property(p => p.HomePlayers).HasColumnName("homePlayers");
            e.Property(p => p.AwayPlayers).HasColumnName("awayPlayers");
            e.Property(p => p.ForfeitedBy).HasColumnName("forfeitedBy");
            e.Property(p => p.GameRuleId).HasColumnName("gameRuleId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(gr => gr.GameRule).WithMany().HasForeignKey(gr => gr.GameRuleId);
            e.HasMany(gr => gr.Awards).WithOne(a => a.GameResult).HasForeignKey(a => a.GameId);
        });

        // ── WinterGameAward ───────────────────────────────────────────────────
        builder.Entity<WinterGameAward>(e =>
        {
            e.ToTable("winter_game_awards");
            e.HasQueryFilter(a => a.Player.DeletedAt == null);
            e.Property(p => p.GameId).HasColumnName("gameId");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.AwardType).HasColumnName("awardType");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(a => a.Player).WithMany().HasForeignKey(a => a.PlayerId);
        });

        // ── WinterSeasonPlayerPayment ─────────────────────────────────────────
        builder.Entity<WinterSeasonPlayerPayment>(e =>
        {
            e.ToTable("winter_season_player_payments");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.PaymentStatus).HasColumnName("paymentStatus");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── WinterSeasonTeamPayment ───────────────────────────────────────────
        builder.Entity<WinterSeasonTeamPayment>(e =>
        {
            e.ToTable("winter_season_team_payments");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.PaymentStatus).HasColumnName("paymentStatus");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── WinterStatMatch ───────────────────────────────────────────────────
        builder.Entity<WinterStatMatch>(e =>
        {
            e.ToTable("winter_stats_matches");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.SeasonPart).HasColumnName("seasonPart");
            e.Property(p => p.MatchId).HasColumnName("matchId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.TeamName).HasColumnName("teamName");
            e.Property(p => p.PointsWon).HasColumnName("pointsWon");
            e.Property(p => p.PointsLost).HasColumnName("pointsLost");
            e.Property(p => p.MatchPoints).HasColumnName("matchPoints");
            e.Property(p => p.HomeMatch).HasColumnName("homeMatch");
            e.Property(p => p.HasScorecard).HasColumnName("hasScorecard");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── WinterStatTeamGame ────────────────────────────────────────────────
        builder.Entity<WinterStatTeamGame>(e =>
        {
            e.ToTable("winter_stats_team_games");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.SeasonPart).HasColumnName("seasonPart");
            e.Property(p => p.MatchId).HasColumnName("matchId");
            e.Property(p => p.GameId).HasColumnName("gameId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.TeamName).HasColumnName("teamName");
            e.Property(p => p.GameType).HasColumnName("gameType");
            e.Property(p => p.NumberOfPlayers).HasColumnName("numberOfPlayers");
            e.Property(p => p.NumberOfPoints).HasColumnName("numberOfPoints");
            e.Property(p => p.IsWon).HasColumnName("isWon");
            e.Property(p => p.IsForfeitGame).HasColumnName("isForfeitGame");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── WinterStatPlayerGame ──────────────────────────────────────────────
        builder.Entity<WinterStatPlayerGame>(e =>
        {
            e.ToTable("winter_stats_player_games");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.SeasonPart).HasColumnName("seasonPart");
            e.Property(p => p.MatchId).HasColumnName("matchId");
            e.Property(p => p.GameId).HasColumnName("gameId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.TeamName).HasColumnName("teamName");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.PlayerName).HasColumnName("playerName");
            e.Property(p => p.PlayerPosition).HasColumnName("playerPosition");
            e.Property(p => p.GameNumber).HasColumnName("gameNumber");
            e.Property(p => p.GameType).HasColumnName("gameType");
            e.Property(p => p.NumberOfPlayers).HasColumnName("numberOfPlayers");
            e.Property(p => p.NumberOfPoints).HasColumnName("numberOfPoints");
            e.Property(p => p.IsWon).HasColumnName("isWon");
            e.Property(p => p.IsForfeit).HasColumnName("isForfeit");
            e.Property(p => p.IsHome).HasColumnName("isHome");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── WinterStatAward ───────────────────────────────────────────────────
        builder.Entity<WinterStatAward>(e =>
        {
            e.ToTable("winter_stats_awards");
            e.Property(p => p.SeasonId).HasColumnName("seasonId");
            e.Property(p => p.SeasonPart).HasColumnName("seasonPart");
            e.Property(p => p.MatchId).HasColumnName("matchId");
            e.Property(p => p.GameId).HasColumnName("gameId");
            e.Property(p => p.TeamId).HasColumnName("teamId");
            e.Property(p => p.TeamName).HasColumnName("teamName");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.PlayerName).HasColumnName("playerName");
            e.Property(p => p.AwardId).HasColumnName("awardId");
            e.Property(p => p.AwardType).HasColumnName("awardType");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── DartEvent ─────────────────────────────────────────────────────────
        builder.Entity<DartEvent>(e =>
        {
            e.ToTable("dart_events");
            e.Property(p => p.EventContact).HasColumnName("eventContact");
            e.Property(p => p.EventContact2).HasColumnName("eventContact2");
            e.Property(p => p.EventTypeId).HasColumnName("eventTypeId");
            e.Property(p => p.EventDate).HasColumnName("eventDate");
            e.Property(p => p.EventEndDate).HasColumnName("eventEndDate");
            e.Property(p => p.DartType).HasColumnName("dartType");
            e.Property(p => p.ImageFileId).HasColumnName("imageFileId");
            e.Property(p => p.PosterFileId).HasColumnName("posterFileId");
            e.Property(p => p.PosterFile).HasColumnName("posterFile");
            e.Property(p => p.FacebookUrl).HasColumnName("facebookUrl");
            e.Property(p => p.HostName).HasColumnName("hostName");
            e.Property(p => p.HostUrl).HasColumnName("hostUrl");
            e.Property(p => p.HostPhone).HasColumnName("hostPhone");
            e.Property(p => p.LocationName).HasColumnName("locationName");
            e.Property(p => p.Address1).HasColumnName("address1");
            e.Property(p => p.Address2).HasColumnName("address2");
            e.Property(p => p.RegistrationStartTime).HasColumnName("registrationStartTime");
            e.Property(p => p.RegistrationEndTime).HasColumnName("registrationEndTime");
            e.Property(p => p.DartStart).HasColumnName("dartStart");
            e.Property(p => p.MapUrl).HasColumnName("mapUrl");
            e.Property(p => p.IsTitleEvent).HasColumnName("isTitleEvent");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasMany(ev => ev.Results).WithOne(r => r.Event).HasForeignKey(r => r.EventId);
        });

        // ── DartEventResult ───────────────────────────────────────────────────
        builder.Entity<DartEventResult>(e =>
        {
            e.ToTable("dart_event_results");
            e.HasQueryFilter(r => r.Event.DeletedAt == null);
            e.Property(p => p.EventId).HasColumnName("eventId");
            e.Property(p => p.SpecificEventName).HasColumnName("specificEventName");
            e.Property(p => p.PlayerId).HasColumnName("playerId");
            e.Property(p => p.OrderId).HasColumnName("orderId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(r => r.Player).WithMany().HasForeignKey(r => r.PlayerId);
        });

        // ── PagePart ──────────────────────────────────────────────────────────
        builder.Entity<PagePart>(e =>
        {
            e.ToTable("page_parts");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        });

        // ── NavGroup ──────────────────────────────────────────────────────────
        builder.Entity<NavGroup>(e =>
        {
            e.ToTable("nav_groups");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasMany(g => g.Items).WithOne(i => i.NavGroup).HasForeignKey(i => i.NavGroupId);
        });

        // ── NavGroupItem ──────────────────────────────────────────────────────
        builder.Entity<NavGroupItem>(e =>
        {
            e.ToTable("nav_group_items");
            e.Property(p => p.NavGroupId).HasColumnName("NavGroupId");
            e.Property(p => p.BrowsableFileId).HasColumnName("BrowsableFileId");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasOne(i => i.BrowsableFile).WithMany().HasForeignKey(i => i.BrowsableFileId);
        });

        // ── NewsPost ──────────────────────────────────────────────────────────
        builder.Entity<NewsPost>(e =>
        {
            e.ToTable("news_posts");
            e.Property(p => p.PostType).HasColumnName("post_type").HasConversion<string>().HasDefaultValue(NewsPostType.News);
            e.Property(p => p.CoverImageId).HasColumnName("cover_image_id");
            e.Property(p => p.PublishedAt).HasColumnName("published_at");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.Property(p => p.DeletedAt).HasColumnName("deleted_at");
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasOne(p => p.CoverImage).WithMany().HasForeignKey(p => p.CoverImageId);
            e.Ignore(p => p.IsDraft);
        });
    }
}
