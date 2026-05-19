# Public: All-posts listing page (/news)

**Type:** AFK
**Blocked by:** Issue 01 — Manage NewsPost CRUD

## What to build

A public Razor Page at `/news` that lists all published NewsPosts ordered by `PublishedAt` descending, with title search and pagination.

Each card in the listing shows:
- Cover image thumbnail (if present)
- Title (links to `/news/{slug}`)
- Published date (formatted)
- Excerpt (if present)

Search: a text input that filters by title using a case-insensitive `LIKE` query. The search term is passed as a query string parameter (`?q=...`) so results are linkable. An empty or missing `q` returns all published posts.

Pagination: show N posts per page (suggested: 10). Page number passed as `?page=...`. If there is only one page, pagination controls are hidden.

Only published posts (`PublishedAt != null`) appear. Drafts and soft-deleted posts are excluded.

## Acceptance criteria

- [ ] `/news` lists all published posts ordered by `PublishedAt` descending
- [ ] Title search filters results — empty search returns all posts
- [ ] Drafts and soft-deleted posts do not appear
- [ ] Each card links to the correct `/news/{slug}` page
- [ ] Pagination works correctly and hides when there is only one page
- [ ] Page is accessible without authentication
- [ ] Empty state is handled gracefully (no posts published yet)

## Blocked by

- Issue 01 — Manage NewsPost CRUD
