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

## Database

PostgreSQL via Npgsql EF Core. All entities use soft deletes (`DeletedAt` nullable column) with a global EF query filter. The original MySQL schema used camelCase column names (e.g. `leagueId`, `homeTeamId`) — the .NET EF model preserves those names via `HasColumnName` overrides.

Raw SQL in `StatsService` uses quoted PascalCase identifiers (e.g. `teams."Name"`) because EF Core creates columns without `HasColumnName` overrides using the C# property name.
