# UI Design Reference

Greater Trenton Dart League — dark members-club aesthetic. All new UI must stay consistent with this system.

---

## Theme

Dark, moody, sports-club feel. Near-black background, warm cream text, gold accents, red/green for wins/losses. Two fonts: **Rajdhani** (display/headings) and **Inter** (body). No rounded-everything softness — deliberate, structured.

---

## Color Palette

| Token | Value | Use |
|---|---|---|
| Background | `#0A0A0B` | Page background |
| Surface | `rgba(255,255,255,0.04)` | Cards, table headers |
| Border | `rgba(255,255,255,0.08)` | Dividers, card borders |
| Border subtle | `rgba(255,255,255,0.06)` | Table row separators |
| Text primary | `#F5ECD7` | Body copy, headings |
| Text muted | `#9CA3AF` | Secondary info, descriptions |
| Text steel | `#6B7280` | Labels, tertiary text, form labels |
| Gold | `#C8962E` | Accents, primary actions, eyebrows |
| Gold hover | `#d9a434` | Gold button hover |
| Red | `#C8101A` | Losses, danger, delete actions |
| Green | `#1A7A3C` | Wins, success |
| Input bg | `#111214` | Form input backgrounds |

**CSS custom properties (defined in `app.css`):**
```css
--color-bg:      #0A0A0B;
--color-surface: rgba(255,255,255,0.04);
--color-border:  rgba(255,255,255,0.08);
--color-text:    #F5ECD7;
--color-muted:   #9CA3AF;
--color-red:     #C8101A;
--color-green:   #1A7A3C;
--color-gold:    #C8962E;
--color-steel:   #6B7280;
```

---

## Typography

**Fonts** (loaded via Google Fonts in `_Layout.cshtml`):
- **Rajdhani** — display, headings, eyebrows, scores. Weights: 500, 600, 700.
- **Inter** — body, labels, data. Weights: 400, 500, 600.

Always set `font-family:'Rajdhani',sans-serif` inline or via the `font-display` variable when using Rajdhani — Tailwind's default font stack won't pick it up.

| Role | Size | Weight | Font | Color | Notes |
|---|---|---|---|---|---|
| Page h1 | `text-3xl` / `text-4xl` | 700 | Rajdhani | `#F5ECD7` | `sm:` breakpoint scale up |
| Page h2 | `text-2xl` | 700 | Rajdhani | `#F5ECD7` | Manage section titles |
| Section h3 | `text-xl` | 700 | Rajdhani | `#F5ECD7` | Card/group headers |
| Eyebrow | `0.6rem` | 600 | Rajdhani | `#C8962E` | Uppercase, 0.22em tracking |
| Body | `text-sm` (0.875rem) | 400 | Inter | `#F5ECD7` | Default content |
| Muted body | `text-sm` | 400 | Inter | `#9CA3AF` | Secondary text |
| Form labels | `text-xs` | 600 | Inter | `#6B7280` | Uppercase, wider tracking |
| Table headers | `text-xs` | 600 | Inter | `#9CA3AF` | Uppercase optional |
| Scores | `text-xl`–`text-2xl` | 700 | Rajdhani | `#F5ECD7` / `#C8962E` | Monospace score values |
| Badges | `text-xs` | 600 | Inter | `#F5ECD7` | Pill badges |

---

## Spacing & Layout

**Page wrapper** (from `_Layout.cshtml`):
```html
<main class="flex-1 w-full max-w-7xl mx-auto px-4 py-6 pb-24 md:pb-6">
```
- Max width: `max-w-7xl`
- Horizontal padding: `px-4`
- Vertical: `py-6`
- Extra bottom padding on mobile (`pb-24`) to clear the fixed bottom nav

**Manage wrapper:**
```html
<main class="flex-1 w-full max-w-7xl mx-auto px-4 py-6">
```

**Common spacing between sections:** `mb-4`, `mb-5`, `mb-6`

**Grid patterns:**
```html
<!-- Stats/cards -->
<div class="grid grid-cols-2 sm:grid-cols-3 gap-3">

<!-- Two-column responsive -->
<div class="grid gap-4 sm:grid-cols-2">

<!-- Three-column -->
<div class="grid gap-4 lg:grid-cols-3">
```

---

## Glass Card

The primary container. Used everywhere for data groups, dashboards, tables, event cards.

```css
.glass-card {
  background: rgba(255,255,255,0.04);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 0.75rem;
}
```

Usage:
```html
<div class="glass-card p-5">
    content
</div>

<!-- Table container -->
<div class="glass-card overflow-x-auto">
    <table>...</table>
</div>

<!-- With section header inside -->
<div class="glass-card overflow-hidden">
    <div class="px-4 py-2" style="border-bottom:1px solid rgba(255,255,255,0.08);background:rgba(255,255,255,0.04)">
        <span class="text-sm font-bold uppercase tracking-wide" style="font-family:'Rajdhani',sans-serif;color:#C8962E">Division A</span>
    </div>
    <table class="w-full text-sm">...</table>
</div>
```

---

## Buttons

### Primary (gold)
```html
<button type="submit" class="px-4 py-2 rounded-lg text-sm font-medium cursor-pointer"
        style="background:#C8962E;color:#0A0A0B;border:none">
    Save
</button>

<!-- As link -->
<a href="/..." class="inline-block px-4 py-2 rounded-lg text-sm font-medium"
   style="background:#C8962E;color:#0A0A0B;text-decoration:none">
    Action
</a>
```

### Secondary (ghost)
```html
<a href="/..." class="inline-block px-4 py-2 rounded-lg text-sm font-medium"
   style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">
    Cancel
</a>
```

### Small (table/inline actions)
```html
<a href="/..." class="inline-block px-2 py-0.5 rounded text-xs font-medium"
   style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">
    Edit
</a>
```

### Danger (delete)
```html
<button type="submit" class="px-2 py-0.5 rounded text-xs font-medium cursor-pointer"
        style="background:#C8101A;color:#fff;border:none">
    Delete
</button>
```

### Danger (larger, e.g. reset stats)
```html
<button type="submit" class="inline-block px-3 py-1.5 rounded text-sm font-medium"
        style="background:#D97706;color:#fff;border:none;cursor:pointer">
    Reset Stats
</button>
```

**Button group pattern** (page action bar):
```html
<div class="flex flex-wrap gap-2 mb-5">
    <a href="..." class="inline-block px-3 py-1.5 rounded text-sm font-medium"
       style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">Back</a>
    <a href="..." class="inline-block px-3 py-1.5 rounded text-sm font-medium"
       style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">Edit</a>
    <!-- danger action -->
    <form method="post" action="..." onsubmit="return confirm('...')" style="display:inline">
        @Html.AntiForgeryToken()
        <button type="submit" class="inline-block px-3 py-1.5 rounded text-sm font-medium"
                style="background:#C8101A;color:#fff;border:none;cursor:pointer">Delete</button>
    </form>
</div>
```

---

## Tables

Standard table inside a glass-card:

```html
<div class="glass-card overflow-x-auto">
<table class="w-full text-sm">
    <thead>
        <tr style="border-bottom:1px solid rgba(255,255,255,0.1);background:rgba(255,255,255,0.04)">
            <th class="px-3 py-2 text-left text-xs font-semibold" style="color:#9CA3AF">Name</th>
            <th class="px-3 py-2 text-left text-xs font-semibold" style="color:#9CA3AF">Role</th>
            <th class="px-3 py-2 text-left text-xs font-semibold" style="color:#9CA3AF">Actions</th>
        </tr>
    </thead>
    <tbody>
    @foreach (var item in Model.Items)
    {
        <tr style="border-bottom:1px solid rgba(255,255,255,0.06)"
            onmouseover="this.style.background='rgba(255,255,255,0.03)'"
            onmouseout="this.style.background=''">
            <td class="px-3 py-2">@item.Name</td>
            <td class="px-3 py-2" style="color:#9CA3AF">@item.Role</td>
            <td class="px-3 py-2">
                <a href="/edit/@item.Id" class="inline-block px-2 py-0.5 rounded text-xs font-medium"
                   style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">Edit</a>
            </td>
        </tr>
    }
    </tbody>
</table>
</div>
```

**Optional search bar above table:**
```html
<div class="flex items-center justify-between mb-3">
    <h3 class="text-lg font-bold" style="font-family:'Rajdhani',sans-serif;color:#F5ECD7">Players</h3>
    <input type="search" placeholder="Search…" autocomplete="off"
           class="rounded-lg px-3 py-1.5 text-sm"
           style="background:#111214;color:#F5ECD7;border:1px solid rgba(255,255,255,0.12);min-width:200px" />
</div>
```

**Win/loss color conventions in cells:**
```html
<td class="px-3 py-2 font-semibold" style="color:#1A7A3C">@item.Wins</td>  <!-- wins -->
<td class="px-3 py-2 font-semibold" style="color:#C8101A">@item.Losses</td> <!-- losses -->
<td class="px-3 py-2 text-xs" style="color:#9CA3AF">@item.Pct</td>          <!-- percentage/muted -->
```

---

## Forms

### Field pattern
```html
<div class="mb-4">
    <label class="block text-xs font-semibold uppercase tracking-wider mb-1" style="color:#6B7280">
        First Name <span style="color:#C8101A">*</span>
    </label>
    <input asp-for="Input.FirstName" class="w-full rounded-lg px-3 py-1.5 text-sm"
           style="background:#111214;color:#F5ECD7;border:1px solid rgba(255,255,255,0.12)" />
    <span asp-validation-for="Input.FirstName" class="text-xs mt-1 block" style="color:#C8101A"></span>
</div>

<!-- Select -->
<div class="mb-4">
    <label class="block text-xs font-semibold uppercase tracking-wider mb-1" style="color:#6B7280">
        Role
    </label>
    <select asp-for="Input.Role" class="w-full rounded-lg px-3 py-1.5 text-sm"
            style="background:#111214;color:#F5ECD7;border:1px solid rgba(255,255,255,0.12)">
        <option value="">— Select —</option>
    </select>
</div>

<!-- Textarea -->
<div class="mb-4">
    <label class="block text-xs font-semibold uppercase tracking-wider mb-1" style="color:#6B7280">
        Notes
    </label>
    <textarea asp-for="Input.Notes" rows="3" class="w-full rounded-lg px-3 py-1.5 text-sm"
              style="background:#111214;color:#F5ECD7;border:1px solid rgba(255,255,255,0.12)"></textarea>
</div>

<!-- Checkbox -->
<div class="mb-4 flex items-center gap-2">
    <input asp-for="Input.IsActive" type="checkbox" class="rounded" />
    <label asp-for="Input.IsActive" class="text-sm" style="color:#F5ECD7">Active</label>
</div>
```

### Form shell with validation summary
```html
<form method="post" class="max-w-xl">
    <div asp-validation-summary="ModelOnly" class="mb-4 p-3 rounded-lg text-sm"
         style="background:rgba(200,16,26,0.15);color:#fca5a5;border:1px solid rgba(200,16,26,0.3)">
    </div>

    <!-- fields -->

    <div class="flex gap-3 mt-6">
        <button type="submit" class="px-4 py-2 rounded-lg text-sm font-medium cursor-pointer"
                style="background:#C8962E;color:#0A0A0B;border:none">Save</button>
        <a href="..." class="inline-block px-4 py-2 rounded-lg text-sm font-medium"
           style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">Cancel</a>
    </div>
</form>
```

---

## Badges / Labels

```html
<!-- Win / success -->
<span class="badge-win">W</span>

<!-- Loss / danger -->
<span class="badge-loss">L</span>

<!-- Neutral -->
<span class="label-default">Pending</span>
```

CSS (in `app.css`):
```css
.badge-win, .label-success {
  display: inline-flex; align-items: center;
  padding: 1px 8px; border-radius: 9999px;
  background-color: #1A7A3C; color: #F5ECD7;
  font-size: 0.75rem; font-weight: 600; letter-spacing: 0.03em;
}
.badge-loss, .label-danger {
  display: inline-flex; align-items: center;
  padding: 1px 8px; border-radius: 9999px;
  background-color: #C8101A; color: #F5ECD7;
  font-size: 0.75rem; font-weight: 600; letter-spacing: 0.03em;
}
.label-default {
  display: inline-flex; align-items: center;
  padding: 1px 8px; border-radius: 9999px;
  background-color: rgba(255,255,255,0.12); color: #F5ECD7;
  font-size: 0.75rem;
}
```

---

## Auth Pages

Auth pages use their own component set, not glass-card. Centered within a radial gold glow.

```html
<div class="auth-wrap">
    <div class="auth-card">
        <div class="auth-eyebrow">Greater Trenton Dart League</div>
        <h1 class="auth-title">Sign In</h1>

        <!-- Validation errors -->
        <div class="auth-error" style="display:none">Error message here</div>

        <form method="post">
            <div class="auth-field">
                <label class="auth-label" asp-for="Email">Email</label>
                <input asp-for="Email" type="email" class="auth-input" autocomplete="email" />
            </div>
            <div class="auth-field">
                <label class="auth-label" asp-for="Password">Password</label>
                <input asp-for="Password" type="password" class="auth-input" />
            </div>

            <button type="submit" class="auth-btn">Sign In</button>
        </form>

        <div class="auth-divider"></div>
        <a href="/Auth/Register" class="auth-btn-ghost">Create an account</a>
    </div>
</div>
```

**Status icon (confirm/error pages):**
```html
<div class="auth-status-icon success">
    <i class="fa-solid fa-check fa-lg"></i>
</div>

<div class="auth-status-icon error">
    <i class="fa-solid fa-xmark fa-lg"></i>
</div>
```

---

## Page Headers

### Public page title
```html
<div class="text-center mb-8">
    <h1 class="text-3xl sm:text-4xl font-bold tracking-wide mb-2"
        style="font-family:'Rajdhani',sans-serif;color:#F5ECD7">
        Season Standings
    </h1>
    <div class="w-16 h-0.5 mx-auto" style="background:#C8101A"></div>
</div>
```

### Manage page title + action bar
```html
<h2 class="text-2xl font-bold mb-4" style="font-family:'Rajdhani',sans-serif;color:#F5ECD7">
    Players
</h2>

<div class="flex flex-wrap gap-2 mb-5">
    <a href="..." class="inline-block px-3 py-1.5 rounded text-sm font-medium"
       style="background:rgba(255,255,255,0.08);color:#F5ECD7;text-decoration:none">Back</a>
    <a href="..." class="inline-block px-3 py-1.5 rounded text-sm font-medium"
       style="background:#C8962E;color:#0A0A0B;text-decoration:none">Add Player</a>
</div>
```

### Section header inside a card
```html
<div class="flex items-center justify-between mb-3">
    <h3 class="text-lg font-bold" style="font-family:'Rajdhani',sans-serif;color:#F5ECD7">
        Teams
    </h3>
    <a href="..." class="inline-block px-2 py-0.5 rounded text-xs font-medium"
       style="background:#C8962E;color:#0A0A0B;text-decoration:none">Add</a>
</div>
```

### Eyebrow + title pattern (cards, events)
```html
<p class="text-xs font-semibold uppercase tracking-widest mb-1" style="color:#C8962E">
    Upcoming Event
</p>
<h3 class="text-xl font-bold mb-1" style="font-family:'Rajdhani',sans-serif">
    Spring Tournament
</h3>
<p class="text-sm" style="color:#9CA3AF">April 12, 2026</p>
```

---

## Navigation

Navigation is handled by `_DefaultNav.cshtml` (public) and `_ManageNav.cshtml` (admin). Both are sticky with `backdrop-filter: blur(12px)`.

**Public nav** uses dropdown groups for Seasons, Players, Teams, Events. Mobile gets a fixed bottom bar (5 icons) plus a hamburger for full menu.

**Manage nav** has two dropdown groups — "Site" (content, nav, files) and "League" (players, teams, seasons, match types).

Do not inline nav HTML in pages — always use the partials.

---

## Scoresheet / Match Display

Defined in `app.css`. Used on match show/edit pages.

```html
<div id="scoresheet">
    <!-- Match header row -->
    <div class="scoresheet-row header">
        <div class="scoresheet-col-team">Home Team</div>
        <div class="scoresheet-col-score">3</div>
        <div class="scoresheet-col-score">2</div>
        <div class="scoresheet-col-team" style="text-align:right">Away Team</div>
    </div>

    <!-- Game group -->
    <div class="score-sheet">
        <div class="game-group-header">501 Doubles</div>
        <div class="game-group">
            <div class="game">
                <div class="game-home-players">Player A / Player B</div>
                <div class="game-result"><span class="badge-win">W</span></div>
                <div class="game-score">3–1</div>
                <div class="game-result"><span class="badge-loss">L</span></div>
                <div class="game-away-players">Player C / Player D</div>
            </div>
        </div>
    </div>
</div>
```

---

## Award Icons

Sprite sheet — positions defined in `app.css` as `.award-img.*` classes.

```html
<div class="award-img high-on" title="High On"></div>
<div class="award-img high-out" title="High Out"></div>
<div class="award-img round-9" title="9-Dart Round"></div>
<div class="award-img t-71" title="Ton 71"></div>
<div class="award-img t-74" title="Ton 74"></div>
<div class="award-img t-77" title="Ton 77"></div>
<div class="award-img t-80" title="Ton 80+"></div>
```

---

## Pagination / Week Navigation

```html
<div class="flex items-center gap-2 mb-6">
    <a href="?week=prev" class="px-3 py-1.5 text-sm rounded-lg transition-colors"
       style="background:rgba(255,255,255,0.06);border:1px solid rgba(255,255,255,0.1);color:#F5ECD7;text-decoration:none">
        &laquo; Prev
    </a>
    <span class="text-sm text-center flex-1" style="color:#9CA3AF">
        <span class="font-semibold" style="color:#F5ECD7">Week 3 of 14</span>
        <span class="mx-1" style="color:#6B7280">—</span>
        January 22, 2026
    </span>
    <a href="?week=next" class="px-3 py-1.5 text-sm rounded-lg transition-colors"
       style="background:rgba(255,255,255,0.06);border:1px solid rgba(255,255,255,0.1);color:#F5ECD7;text-decoration:none">
        Next &raquo;
    </a>
</div>
```

---

## Icons

Font Awesome 6.5.0 loaded from cdnjs. Use `fa-solid` for solid style.

Common icons in use:
```html
<i class="fa-solid fa-ranking-star" style="color:#C8962E"></i>   <!-- standings -->
<i class="fa-solid fa-calendar-days" style="color:#C8962E"></i>  <!-- schedule -->
<i class="fa-solid fa-chart-bar" style="color:#C8962E"></i>      <!-- stats -->
<i class="fa-solid fa-trophy" style="color:#C8962E"></i>         <!-- awards -->
<i class="fa-brands fa-facebook" style="color:#C8962E"></i>      <!-- social -->
<i class="fa-solid fa-check fa-lg"></i>                          <!-- success -->
<i class="fa-solid fa-xmark fa-lg"></i>                          <!-- error -->
```

Icon color in nav/accent contexts: `#C8962E` (gold). In body text, inherit from parent.

---

## Manage Dashboard Cards

Used on `Manage/Index.cshtml` for quick-access stat tiles.

```html
<div class="grid grid-cols-2 sm:grid-cols-3 gap-3 max-w-lg mb-8">
    <a href="/manage/..." class="glass-card p-4 flex flex-col items-center gap-1"
       style="color:#F5ECD7;text-decoration:none"
       onmouseover="this.style.background='rgba(255,255,255,0.08)'"
       onmouseout="this.style.background=''">
        <span class="text-2xl font-bold" style="font-family:'Rajdhani',sans-serif;color:#C8962E">
            @Model.Count
        </span>
        <span class="text-sm font-medium">Players</span>
    </a>
</div>
```

---

## Utility Classes

```css
.no-print          /* hidden in print view */
.clickable-row     /* cursor:pointer for JS-interactive rows */
.validation-error  /* red bold validation text */
.forfeit           /* red pill badge for forfeited games */
.loss-column       /* subtle red row background (#C8101A 12%) */
.team-stat-row     /* bold stats summary row */
```

---

## Animations & Effects

**Auth card entrance:**
```css
@keyframes auth-card-in {
  from { opacity: 0; transform: translateY(14px); }
  to   { opacity: 1; transform: translateY(0); }
}
/* applied via: animation: auth-card-in 0.4s cubic-bezier(0.16,1,0.3,1) both */
```

**Hover transitions:** Use `transition-colors` (Tailwind) or `transition: background 0.15s` for interactive elements.

**Backdrop blur:** `backdrop-filter: blur(12px)` on sticky navs and dropdowns.

**Dropdown shadow:** `box-shadow: 0 8px 32px rgba(0,0,0,0.5)`.

**Text shadows (hero header only):**
- Large title: `text-shadow: 0 2px 12px rgba(0,0,0,0.8)`
- Sub-title: `text-shadow: 0 2px 8px rgba(0,0,0,0.8)`

---

## Header / Hero

The site header uses a photo background with dark gradient overlay. Do not replicate this in sub-pages — it's a layout-level element in `_Layout.cshtml`.

```html
<header style="border-bottom:1px solid rgba(255,255,255,0.08);
               background:linear-gradient(rgba(10,10,11,0.62) 0%,rgba(10,10,11,0.75) 60%,rgba(10,10,11,0.95) 100%),
                          url('/images/trenton-makes.jpg') center 40%/cover no-repeat">
    <div class="max-w-7xl mx-auto px-4 py-10 text-center">
        <span class="block text-4xl sm:text-5xl font-bold tracking-widest uppercase"
              style="font-family:'Rajdhani',sans-serif;color:#F5ECD7;text-shadow:0 2px 12px rgba(0,0,0,0.8)">
            Greater Trenton
        </span>
        <span class="block text-lg sm:text-xl font-semibold tracking-[0.2em] uppercase"
              style="font-family:'Rajdhani',sans-serif;color:#C8962E;text-shadow:0 2px 8px rgba(0,0,0,0.8)">
            Dart League
        </span>
    </div>
</header>
```

---

## Responsive Conventions

- **Mobile-first.** All Tailwind classes apply at mobile; prefix with `sm:` (640px) or `md:` (768px) for larger screens.
- **Bottom nav** on mobile is fixed. All public pages need `pb-24 md:pb-6` on main content to avoid overlap.
- **Manage pages** have no bottom nav — use `py-6` only.
- **Nav hidden/shown:** `hidden md:flex` (desktop only), `flex md:hidden` (mobile only).
- **Typography scale:** `text-3xl sm:text-4xl` for h1, `text-4xl sm:text-5xl` for the hero header.

---

## Dos and Don'ts

**Do:**
- Use `glass-card` as the default container for any grouped content
- Use Rajdhani for all headings, scores, and eyebrows
- Keep action buttons gold (`#C8962E`) for primary, semi-transparent white for secondary
- Use `rgba` borders and surfaces — never solid white or gray dividers
- Keep form inputs on `#111214` background with `rgba(255,255,255,0.12)` border

**Don't:**
- Use white or light backgrounds anywhere
- Use non-Rajdhani fonts for headings
- Use blue for any accent or link color — this site has no blue
- Add box shadows to cards (the backdrop blur + border is the depth signal)
- Deviate from the red/green win-loss convention
