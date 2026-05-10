# TrentonDarts (.NET)

ASP.NET Core 10 Razor Pages app managing the Greater Trenton Dart League (GTDL). Port of the original Laravel 5.2 application.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL and MinIO)
- [Node.js 18+](https://nodejs.org/) (for the Vite frontend build)

## Local Development Setup

### 1. Start infrastructure

```powershell
docker compose up -d
```

This starts:
- **PostgreSQL** on `localhost:5432`
- **MinIO** (S3-compatible object storage) on `localhost:9000`, console at `localhost:9001`

### 2. Set up MinIO bucket

1. Open http://localhost:9001
2. Log in with `minioadmin` / `minioadmin`
3. Create a bucket named **`gtdl`**
4. Set the bucket's **Access Policy → Public**

This only needs to be done once. The `minio_data` Docker volume persists across restarts.

### 3. Apply database migrations

```powershell
dotnet ef database update --project TrentonDarts.Web
```

### 4. Build the frontend

```powershell
cd TrentonDarts.Web/ClientApp
npm install
npm run build
```

For watch mode during development:

```powershell
npm run dev
```

### 5. Run the app

```powershell
dotnet run --project TrentonDarts.Web
```

App is available at:
- http://localhost:5042
- https://localhost:7062

### 6. Create an admin account

Navigate to `/Auth/Register`. The first user to register automatically gets the **Admin** role.

---

## Project Structure

| Folder | Purpose |
|---|---|
| `TrentonDarts.Web/Data/` | `AppDbContext`, all EF entities, `StorageOptions` |
| `TrentonDarts.Web/Domain/` | `MatchRepository` — match aggregate + scoring rules |
| `TrentonDarts.Web/Services/` | Scoped services: `NavService`, `SeasonService`, `StatsService`, `BrowsableFileService`, `FileStorageService`, etc. |
| `TrentonDarts.Web/Pages/` | Razor Pages; `/Manage` subtree requires authentication |
| `TrentonDarts.Web/Authorization/` | `LaravelPasswordHasher` for legacy BCrypt `$2y$` hashes |
| `TrentonDarts.Web/ClientApp/` | Vite 6 + Sass; output goes to `wwwroot/dist` |
| `TrentonDarts.Web/Migrations/` | EF Core migrations |
| `TrentonDarts.Tests/` | xUnit tests |

---

## Common Commands

### Backend

```powershell
dotnet run --project TrentonDarts.Web               # Run
dotnet test                                          # Run tests
dotnet ef migrations add <Name> --project TrentonDarts.Web   # New migration
dotnet ef database update --project TrentonDarts.Web         # Apply migrations
```

### Frontend (from `TrentonDarts.Web/ClientApp`)

```powershell
npm run build   # One-time build → wwwroot/dist
npm run dev     # Watch mode
```

### Docker

```powershell
docker compose up -d        # Start services
docker compose down         # Stop (data persisted)
docker compose down -v      # Stop and wipe all volumes (fresh DB + storage)
```

---

## File / Document Storage

Uploaded files are stored in S3-compatible object storage. Locally this is MinIO; in production it targets DigitalOcean Spaces.

### Managing documents

Admin users can upload PDFs and other documents at `/Manage/{leagueId}/documents`. Uploaded documents appear in the site's **Other** navigation menu automatically.

### Production (DigitalOcean Spaces)

Set the following via environment variables or `dotnet user-secrets`:

| Key | Example value |
|---|---|
| `Storage__ServiceUrl` | `https://nyc3.digitaloceanspaces.com` |
| `Storage__BucketName` | `gtdl` |
| `Storage__AccessKey` | *(key ID from DO API → Spaces Keys)* |
| `Storage__SecretKey` | *(secret key)* |
| `Storage__PublicBaseUrl` | `https://gtdl.nyc3.digitaloceanspaces.com` |
| `Storage__Region` | `nyc3` |
| `Storage__ForcePathStyle` | `false` |

The Space itself should be set to **public** so uploaded files are directly accessible.

---

## Authentication

Uses ASP.NET Core Identity with a custom `LaravelPasswordHasher` that accepts the legacy `$2y$` BCrypt hashes from the original PHP app. Do not swap out the password hasher without a migration plan for existing users.

### Roles

| Role | Description |
|---|---|
| `Admin` | Full management access; first registered user gets this role |
| `Owner` | Reserved for league owner |
| `BoardMember` | Board-level access |
| `Member` | League member |
| `User` | Default role for new registrations |

---

## Data Migration (MySQL → PostgreSQL)

The script `Tools/migrate-mysql-to-postgres.ps1` converts a phpMyAdmin MySQL dump of the original Laravel database into a PostgreSQL-compatible SQL file.

### One-time import

```powershell
# 1. Export from MySQL (phpMyAdmin: Export → Quick → SQL, or via CLI)
mysqldump -u root -p trentondarts > C:\Users\johnn\Downloads\trentondarts_export.sql

# 2. Generate the PostgreSQL SQL file (~20 min — large stats tables)
.\Tools\migrate-mysql-to-postgres.ps1

# 3. Apply to PostgreSQL
psql -U postgres -d trentondarts -f '.\Tools\migration-output.sql'
```

### Re-importing updated data

If you need to refresh from a newer MySQL export, truncate the existing data first:

```sql
SET session_replication_role = replica;
TRUNCATE TABLE winter_stats_awards, winter_stats_player_games, winter_stats_team_games,
  winter_stats_matches, winter_season_team_payments, winter_season_player_payments,
  winter_game_awards, winter_game_results, winter_match_results, winter_season_matches,
  winter_season_team_players, winter_season_teams, winter_season_weeks, winter_seasons,
  match_type_game_rules, dart_event_results, dart_events, page_parts, browsable_files,
  board_members, teams, players, match_types, sponsors CASCADE;
SET session_replication_role = DEFAULT;
```

Then re-run steps 1–3 above. To use a different source file:

```powershell
.\Tools\migrate-mysql-to-postgres.ps1 -SourceFile "C:\path\to\export.sql"
```

### What the script handles

- Column renames from MySQL camelCase to EF PascalCase (e.g. `name` → `"Name"`, `id` → `"Id"`)
- Boolean conversion: MySQL `TINYINT(1)` `0`/`1` → PostgreSQL `false`/`true`
- Invalid MySQL timestamps (`'0000-00-00 00:00:00'`, `'0000-00-00'`) → `'1970-01-01 00:00:00'`
- NULL timestamps in non-nullable columns → `'1970-01-01 00:00:00'`
- `userId` forced to `NULL` in `players` and `board_members` (no Identity mapping)
- `leagueId` injected as `1` where absent from MySQL data
- `division` excluded from `winter_stats_*` tables (not in EF entities)
- Sequence resets after import so new rows don't conflict with migrated IDs

> **Note:** Do not run multiple instances of the script simultaneously — the 101K-row `winter_stats_player_games` table requires significant memory and concurrent runs will crash.

---

## Database

PostgreSQL via Npgsql EF Core. All entities use soft deletes (`DeletedAt` nullable column) with a global EF query filter. The original MySQL schema used camelCase column names (e.g. `leagueId`, `homeTeamId`) — the .NET EF model preserves those names via `HasColumnName` overrides.

Raw SQL in `StatsService` uses quoted PascalCase identifiers (e.g. `teams."Name"`) because EF Core creates columns without `HasColumnName` overrides using the C# property name.
