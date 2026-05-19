# ADR 0001: NewsPost replaces PagePart as the front-page bulletin

## Status
Accepted

## Context
The front page League Bulletin section previously displayed two singleton `PagePart` records — `MainPageHeader` and `GTDL_PLAYER_OF_THE_WEEK` — as named HTML blobs queried by name. This model has no publish workflow, no ordering, and no way to surface historical items.

The league wanted to post ongoing news (announcements, recaps, etc.) with images, drafts, and a searchable archive.

## Decision
Replace the League Bulletin with a feed of the 3 most recently published `NewsPost` records. `PagePart` is retained in the schema and Manage UI but is no longer queried on the front page.

## Consequences
- `PagePart` rows `MainPageHeader` and `GTDL_PLAYER_OF_THE_WEEK` are orphaned — no page renders them. They are kept to avoid a destructive migration and in case static content blocks are needed elsewhere in the future.
- The front page now requires at least one published `NewsPost` to show anything in the bulletin area. An empty state should be handled gracefully.
- The `PageParts` Manage section remains functional but has no visible effect on the public site until explicitly wired to a new use.
