# Manage: NewsPost cover image upload

**Type:** AFK
**Blocked by:** Issue 01 — Manage NewsPost CRUD

## What to build

Extend the NewsPost Create and Edit forms to support uploading a cover image. The cover image is stored in S3 via the existing `BrowsableFileService` and the resulting `BrowsableFile` ID is saved to `NewsPost.CoverImageId`.

On the Create form: an optional file input. If a file is provided, it is uploaded before the post is saved and the resulting `BrowsableFile.Id` is written to `CoverImageId`.

On the Edit form: show the current cover image as a thumbnail (using `BrowsableFile.RelativePath` or `PublicBaseUrl`). Provide a file input to replace it. Provide a "remove cover image" option that sets `CoverImageId` to null (the existing `BrowsableFile` record is left intact).

Inline images within the TipTap body are out of scope for this slice — the editor already supports pasting/dragging images via base64 or external URLs.

## Acceptance criteria

- [ ] Create form accepts an optional image file and uploads it via `BrowsableFileService`
- [ ] Saved post has `CoverImageId` pointing to the newly uploaded `BrowsableFile`
- [ ] Edit form shows current cover image thumbnail if one exists
- [ ] Replacing the image on edit uploads a new file and updates `CoverImageId`
- [ ] "Remove cover image" clears `CoverImageId` without deleting the `BrowsableFile` row
- [ ] Non-image file types are rejected with a validation error
- [ ] No cover image is a valid state — forms work correctly when `CoverImageId` is null

## Blocked by

- Issue 01 — Manage NewsPost CRUD
