# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Local database (Docker)
```powershell
docker compose up -d        # Start PostgreSQL on localhost:5432
docker compose down         # Stop (data persisted in postgres_data volume)
docker compose down -v      # Stop and wipe the volume (fresh DB)
```
After starting the container, run `dotnet ef database update` once to apply all migrations.

### Backend
```powershell
dotnet run --project TrentonDarts.Web          # Run the app (localhost:5042 / 7062)
dotnet test                                     # Run xUnit tests
dotnet ef migrations add <Name> --project TrentonDarts.Web   # Add EF migration
dotnet ef database update --project TrentonDarts.Web         # Apply migrations
```

### Frontend (run from `TrentonDarts.Web/ClientApp`)
```powershell
npm run build   # One-time Vite build → wwwroot/dist
npm run dev     # Watch mode (Vite --watch)
```

### Single test
```powershell
dotnet test --filter "FullyQualifiedName~MyTestClass"
```

## Architecture

ASP.NET Core 10 **Razor Pages** app managing a dart league. No MediatR — thin pages call scoped services directly.

### Solution layout
- `TrentonDarts.Web` — main app (Razor Pages + EF Core + Identity)
- `TrentonDarts.Tests` — xUnit tests referencing the web project

### Key layers inside `TrentonDarts.Web`

| Folder | Role |
|---|---|
| `Data/` | `AppDbContext` (IdentityDbContext), all entities, EF Fluent config |
| `Domain/` | `MatchRepository` — complex match aggregate + scoring rules |
| `Services/` | `SeasonService`, `StatsService`, `PlayerHistoryService`, `UpdateMatchStatsService` |
| `Authorization/` | `LaravelPasswordHasher` — converts `$2y$` → `$2b$` for legacy users |
| `Pages/` | Razor Pages; `/Manage` subtree requires authenticated admin |

### Database
PostgreSQL via Npgsql EF Core. All entities use soft deletes — `AppDbContext` registers a global query filter on `DeletedAt`. Always check when writing raw queries or new filtered includes.

### Frontend
Vite 6 + Sass. Entry points in `ClientApp/`; output goes to `wwwroot/dist`. No heavy JS framework — mostly progressive enhancement.

### Authentication
ASP.NET Core Identity with the custom `LaravelPasswordHasher` registered as the `IPasswordHasher<IdentityUser>` implementation. Don't swap this out without a migration plan for existing users.

### Stats / scoring
`UpdateMatchStatsService` is called after every match save to recalculate denormalized stats. `SeasonService` returns records (`DivStanding`, `DivWeek`, etc.) — keep these as value objects, don't add mutable state.

---

## Laravel source reference (`C:\dev\trentondarts`)

The .NET app is a port of a **Laravel 5.2 / PHP** project. The original is at `C:\dev\trentondarts`. Consult it when something looks incomplete or missing in the .NET version.

### Original tech stack
- PHP ≥ 5.5.9, Laravel 5.2, MySQL (camelCase column names)
- Frontend: React 0.13 + Bootstrap 3, built with Gulp/Browserify
- Auth: Laravel `Authenticate` middleware (BCrypt `$2y$` hashes — why `LaravelPasswordHasher` exists)

### Bootstrapping the Laravel app
```bash
composer install
npm install
gulp                     # compiles SASS + React bundles → public/
php artisan migrate
php artisan key:generate
```
Environment variables are in `.env` (copy `.env.example`). Database is MySQL; connection key is `DB_DATABASE`.

### Route map (Laravel → .NET equivalents)

| Laravel route | .NET Razor Page |
|---|---|
| `GET /` | `Pages/Index` |
| `GET /season/{id}` (show) | `Pages/Seasons/Show` |
| `GET /season/{id}/schedule` | `Pages/Seasons/Schedule` |
| `GET /season/{id}/stats` | `Pages/Seasons/Stats` |
| `GET /season/{id}/awards` | `Pages/Seasons/Awards` |
| `GET /season/{id}/leaderboard` | `Pages/Seasons/Leaderboard` |
| `GET /season/{id}/match/{id}` | `Pages/Matches/Show` |
| `GET /season/{id}/match/{id}/edit` | `Pages/Matches/Edit` |
| `GET /player/{id}` | `Pages/Players/Show` |
| `GET /event/results` | `Pages/Events/Results` |
| `GET /sponsor` | `Pages/Sponsors/Index` (or similar) |
| `/manage/{leagueId}/**` | `Pages/Manage/**` |
| `GET /auth/login` | `Pages/Auth/Login` |

The Laravel app also has **team payments**, **player payments**, and **board members** under `/manage` — check whether those pages have been ported yet.

### Domain structure (original bounded contexts → .NET folders)

| Laravel namespace | .NET equivalent |
|---|---|
| `app/LeagueManagement/Models/` | `Data/` entities + `Pages/Manage/` |
| `app/MatchDomain/Models/` | `Domain/` (MatchRepository + scoring) |
| `app/Stats/Models/` | `Services/StatsService`, `UpdateMatchStatsService` |
| `app/SiteManagement/Models/` | `Data/` (DartEvent, PagePart entities) |

### Key schema notes
- Original DB is **MySQL with camelCase column names** (e.g., `leagueId`, `homeTeamId`). The .NET EF model uses the same names — don't silently snake_case them.
- The `game_results` table stores `homePlayers` and `awayPlayers` as **semicolon-delimited ID strings** and `legs` as a serialized string — match scoring relies on deserialization into `MatchResult`/`GameResult` domain objects (snapshot pattern).
- `stats_*` tables are **denormalized caches** rebuilt by `updateMatchStats` job after every scorecard save. Never treat them as a source of truth.
- Soft deletes on `teams`, `players`, `winter_seasons` — same as .NET `DeletedAt` filter.
- SQL reference files in `C:\dev\trentondarts\`: `original-db.sql` (full schema), `season1.sql`–`season3.sql` (season data), `TestSetupData.sql`.

### Features to verify are ported
- [ ] Team payment tracking (`season_team_payments`)
- [ ] Player payment tracking (`season_player_payments`)
- [ ] Board member management
- [ ] Stats export / awards export (CSV download routes)
- [ ] `BrowsableFile` / file library
- [ ] `PagePart` CMS content blocks
- [ ] Dart event management + results
- [ ] Match type + game rule configuration (`match_types`, `match_type_game_rules`)
