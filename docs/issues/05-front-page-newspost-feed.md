# Front page: Replace League Bulletin with NewsPost feed

**Type:** AFK
**Blocked by:** Issue 01 (Manage CRUD), Issue 03 (post detail page — cards must link somewhere)

## What to build

Replace the League Bulletin section on the home page. Currently it renders two `PagePart` singleton HTML blobs (`MainPageHeader`, `GTDL_PLAYER_OF_THE_WEEK`). Replace this with a feed of the 3 most recently published NewsPosts.

Per ADR 0001: `PagePart` rows are retained in the database and the Manage UI remains functional, but the front page no longer queries them.

Each card in the feed shows:
- Cover image (if present)
- Title (links to `/news/{slug}`)
- Published date (formatted)
- Excerpt (if present)

The feed shows a maximum of 3 posts. If fewer than 3 posts are published, show only what exists. If no posts are published, display a graceful empty state (e.g., a placeholder message — no broken layout).

The `Index` page model removes the `PagePart` queries for `MainPageHeader` and `GTDL_PLAYER_OF_THE_WEEK` and replaces them with a query for the 3 most recent published NewsPosts.

## Acceptance criteria

- [ ] League Bulletin section renders the 3 most recent published NewsPosts
- [ ] Each card links correctly to `/news/{slug}`
- [ ] Fewer than 3 published posts renders without layout errors
- [ ] Zero published posts shows a graceful empty state
- [ ] `PagePart` queries for `MainPageHeader` and `GTDL_PLAYER_OF_THE_WEEK` are removed from `Index` page model
- [ ] Existing bulletin items (title event card) are unaffected

## Blocked by

- Issue 01 — Manage NewsPost CRUD
- Issue 03 — Public post detail page (cards link to `/news/{slug}`)
