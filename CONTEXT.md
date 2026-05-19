# Domain Glossary

## ScorecardService

The service that coordinates submitting a match scorecard. Owns the sequencing of: persisting the scorecard data and synchronizing all derived stat tables. Callers call one method; they do not manage the two-step sequence themselves.

**Method**: `SubmitScorecardAsync(int matchId, ScorecardSaveDto data)`

**Contrast with**: `MatchRepository` (persistence of the match aggregate), `UpdateMatchStatsService` (stat derivation — called by ScorecardService, not by callers directly).

`UpdateMatchStatsService.UpdateAsync` accepts a `MatchResult` directly (not a `matchId`), so `ScorecardService` loads the aggregate once and passes it through — no second fetch. The `ResetStats` page loads via `MatchRepository` and passes the result in explicitly.

## MatchResult

The domain aggregate for a played match. Constructed only via `MatchResult.From(MatchResultSnapshot, MatchRules)` — the factory is the sole public construction path. `LoadSnapshot` and `LoadRules` are private implementation details. A `MatchResult` returned by the factory is always fully initialized; invalid intermediate state is unreachable.

**Contrast with**: `MatchResultSnapshot` (the flat data bag used for serialization and persistence).

## SeasonPart

An enum with three values: `Pre`, `Regular`, `Post`. Stored in the DB as lowercase strings (`"pre"`, `"regular"`, `"post"`) via an EF value converter. No migration needed — the column type stays VARCHAR.

`SeasonPart?` (nullable) is used as a filter parameter throughout `StatsService` and `SeasonService`. Null means "no filter" (whole-season view). The string sentinel `"whole"` is not a domain concept and does not exist after this change.

`WinterSeasonWeek.WeekType` is typed as `SeasonPart` (same three values).

**Division is NOT a value object** — it remains a plain string (e.g., `"A1"`, `"B2"`). The tier letter is extracted inline where needed.

## Award

An award belongs to a specific game and a specific player. **Invariant**: the award player is always one of the players listed in that game (`GameResult.HomePlayers` or `GameResult.AwayPlayers`). Team attribution for an award is derived from the game's player lists — not from the season roster. There is no concept of a match-level award assigned outside the game's participants.
