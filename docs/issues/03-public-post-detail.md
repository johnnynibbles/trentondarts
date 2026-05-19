# Public: Individual post page (/news/{slug})

**Type:** AFK
**Blocked by:** Issue 01 — Manage NewsPost CRUD

## What to build

A public Razor Page at `/news/{slug}` that renders a single published NewsPost.

The page displays:
- Cover image (if present) as a full-width or hero image
- Title
- Published date (formatted)
- Excerpt (if present) as a subtitle or lead paragraph
- Full TipTap HTML body rendered as raw HTML

Access rules:
- If the slug matches a published post (`PublishedAt != null`), render it
- If the slug matches a draft or soft-deleted post, return 404
- If no post matches the slug at all, return 404

No authentication required — the page is fully public.

## Acceptance criteria

- [ ] Published post renders at `/news/{slug}` with cover image, title, date, and body
- [ ] Draft post (PublishedAt == null) returns 404
- [ ] Soft-deleted post returns 404
- [ ] Unknown slug returns 404
- [ ] Page is accessible without authentication

## Blocked by

- Issue 01 — Manage NewsPost CRUD
