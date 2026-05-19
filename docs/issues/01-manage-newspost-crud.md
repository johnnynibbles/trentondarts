# Manage: NewsPost CRUD

**Type:** AFK
**Blocked by:** None — can start immediately

## What to build

Introduce the `NewsPost` entity end-to-end: schema through admin UI. This slice delivers a fully working admin interface for creating, editing, publishing, and deleting NewsPosts — with the database migration included.

**Schema** (included in this slice):

```
NewsPost
  Id            int          PK, identity
  Title         string       required
  Slug          string       required, unique index
  Excerpt       string?      optional plain-text summary
  Html          string?      TipTap rich-text body
  CoverImageId  int?         FK → BrowsableFile (nullable, added in a later slice)
  PublishedAt   DateTime?    null = draft, non-null = published
  CreatedAt     DateTime
  UpdatedAt     DateTime
  DeletedAt     DateTime?    soft delete
```

Register a global soft-delete query filter (`DeletedAt == null`) consistent with the existing `DartEvent` pattern. Apply the EF migration as part of this slice.

**Admin pages** at `/Manage/{leagueId}/news/` — requires Owner or Admin role:

- **List** — all non-deleted posts (drafts + published), ordered by `CreatedAt` descending. Shows title, slug, published status, and action links.
- **Create** — title (required), slug (auto-generated from title, editable), excerpt (optional), TipTap HTML body, publish toggle.
- **Edit** — loads existing post, same fields. Publish toggle sets `PublishedAt = DateTime.UtcNow`; unpublish clears it to null.
- **Delete** — soft delete via `OnPostDeleteAsync` (sets `DeletedAt = DateTime.UtcNow`).

No cover image in this slice — that comes in the next slice.

## Acceptance criteria

- [ ] Migration applies cleanly against a fresh database
- [ ] `/Manage/{leagueId}/news` lists all non-deleted posts (drafts + published)
- [ ] Create form saves a new post with slug auto-generated from title
- [ ] Edit form loads and saves all fields including publish/unpublish
- [ ] Slug uniqueness is validated — duplicate slugs are rejected with a model error
- [ ] Soft delete removes post from public visibility without destroying the row
- [ ] All pages require Owner or Admin role

## Blocked by

None — can start immediately
