# UI Design Reference

Greater Trenton Dart League — modern-minimal light theme. All new UI must stay consistent with this system.

---

## Theme

Light, clean, modern. Near-white background, dark text, blue accent, red/green for wins/losses. System fonts throughout — no web font downloads. Sharp, structured layout with thin borders and no blur effects.

---

## Color Palette

| Token | Value | Use |
|---|---|---|
| Background | `#F9FAFB` | Page background |
| Surface | `#FFFFFF` | Cards, widgets, panels |
| Border | `#E5E7EB` | Dividers, card borders |
| Border subtle | `#F3F4F6` | Table row separators |
| Text primary | `#111827` | Body copy, headings |
| Text muted | `#6B7280` | Secondary info, descriptions |
| Text steel | `#9CA3AF` | Labels, tertiary text |
| Accent | `#2563EB` | Primary actions, links, active states |
| Accent hover | `#1D4ED8` | Accent button hover |
| Accent bg | `#EFF6FF` | Accent highlight background |
| Accent border | `#BFDBFE` | Accent highlight border |
| Red | `#DC2626` | Losses, danger, delete actions |
| Red bg | `#FEF2F2` | Danger badge/alert background |
| Green | `#16A34A` | Wins, success |
| Green bg | `#F0FDF4` | Win badge/success background |
| Input bg | `#F9FAFB` | Form input backgrounds |
| Input border | `#D1D5DB` | Form input borders |

**CSS custom properties (defined in `app.css`):**
```css
--color-bg:      #F9FAFB;
--color-surface: #FFFFFF;
--color-border:  #E5E7EB;
--color-text:    #111827;
--color-muted:   #6B7280;
--color-red:     #DC2626;
--color-green:   #16A34A;
--color-gold:    #2563EB;   /* repurposed as accent/blue */
--color-steel:   #9CA3AF;
```

---

## Typography

**Fonts:** System font stack — no external font load required.
```css
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', system-ui, sans-serif;
font-family: 'JetBrains Mono', 'IBM Plex Mono', ui-monospace, Menlo, monospace; /* for scores/numbers */
```

| Role | Size | Weight | Color | Notes |
|---|---|---|---|---|
| Page h1 | `clamp(24px, 3.5vw, 38px)` | 800 | `#111827` | `letter-spacing: -0.03em` |
| Page h2 | `17px` | 700 | `#111827` | Section titles |
| Section h3 | `15px` | 700 | `#111827` | Card/panel headers |
| Eyebrow/label | `11px–12px` | 700 | `#6B7280` | Uppercase, wide tracking |
| Body | `15px–16px` | 400 | `#111827` | Default content, `line-height: 1.65` |
| Muted body | `13px–14px` | 400 | `#6B7280` | Secondary text |
| Form labels | `13px` | 600 | `#111827` | Normal case |
| Table headers | `10px` | 700 | `#6B7280` | Uppercase, `letter-spacing: 0.06em` |
| Scores | `18px–28px` | 800 | `#111827` | Monospace, tabular nums |
| Badges/tags | `11px` | 700 | varies | Pill or square with bg color |

---

## Spacing & Layout

**Page wrapper** (from `_Layout.cshtml`):
```html
<main class="flex-1 w-full max-w-7xl mx-auto px-4 py-6 pb-24 md:pb-6">
```
- Max width: `max-w-7xl` (or `--max-w: 1200px` in custom CSS)
- Horizontal padding: `px-4` (24px on desktop)
- Vertical: `py-6`

**Two-column layout (homepage / article):**
```html
<!-- Main content + sidebar -->
<div class="main-wrap">  <!-- 1fr + 340px on desktop -->
  <div class="posts-col">...</div>
  <aside class="sidebar">...</aside>
</div>
```

**Common spacing between sections:** `mb-4`, `mb-5`, `mb-6`, `gap: 20px–24px`

**Grid patterns:**
```html
<!-- Stats/cards -->
<div style="display:grid;gap:12px;grid-template-columns:repeat(auto-fill,minmax(160px,1fr))">

<!-- Events grid -->
<div style="display:grid;gap:12px;grid-template-columns:repeat(auto-fill,minmax(240px,1fr))">

<!-- Two-column responsive -->
<div class="grid gap-4 sm:grid-cols-2">
```

---

## Card / Widget

The primary container. Light background, thin border, rounded corners.

```css
.card {
  background: #FFFFFF;
  border: 1px solid #E5E7EB;
  border-radius: 10px;
  overflow: hidden;
}
```

The `.glass-card` class has been updated to the same values (no blur, light background).

**Card with header:**
```html
<div class="glass-card overflow-hidden">
  <div style="padding:14px 18px;border-bottom:1px solid #E5E7EB;display:flex;align-items:center;justify-content:space-between;">
    <span style="font-size:14px;font-weight:700;color:#111827">Section Title</span>
    <span style="font-size:11px;padding:2px 8px;border-radius:100px;background:#F3F4F6;color:#6B7280;border:1px solid #E5E7EB">Badge</span>
  </div>
  <!-- content -->
</div>
```

**Section header inside a card:**
```html
<div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:20px">
  <h2 style="font-size:17px;font-weight:700;letter-spacing:-0.02em;color:#111827">Teams</h2>
  <a href="..." style="font-size:13px;color:#2563EB;font-weight:500;text-decoration:none">All posts →</a>
</div>
```

---

## Buttons

### Primary (blue/accent)
```html
<a href="/..." class="inline-flex items-center gap-1.5 px-4 py-2 rounded text-sm font-semibold"
   style="background:#2563EB;color:#fff;text-decoration:none;border:1px solid #2563EB">
    View Standings
</a>

<button type="submit" class="px-4 py-2 rounded text-sm font-semibold cursor-pointer"
        style="background:#2563EB;color:#fff;border:1px solid #2563EB">
    Save
</button>
```

### Ghost / Secondary
```html
<a href="/..." class="inline-flex items-center px-4 py-2 rounded text-sm font-medium"
   style="background:transparent;color:#111827;border:1px solid #E5E7EB;text-decoration:none">
    Cancel
</a>
```

### Small (table/inline actions)
```html
<a href="/..." class="inline-block px-2 py-0.5 rounded text-xs font-medium"
   style="background:#F3F4F6;color:#374151;border:1px solid #E5E7EB;text-decoration:none">
    Edit
</a>
```

### Danger (delete)
```html
<button type="submit" class="px-2 py-0.5 rounded text-xs font-medium cursor-pointer"
        style="background:#DC2626;color:#fff;border:none">
    Delete
</button>
```

**Button group pattern:**
```html
<div class="flex flex-wrap gap-2 mb-5">
  <a href="..." class="inline-flex items-center px-3 py-1.5 rounded text-sm font-medium"
     style="background:transparent;color:#111827;border:1px solid #E5E7EB;text-decoration:none">Back</a>
  <a href="..." class="inline-flex items-center px-3 py-1.5 rounded text-sm font-medium"
     style="background:#2563EB;color:#fff;border:1px solid #2563EB;text-decoration:none">Add Player</a>
</div>
```

---

## Tables

Standard table inside a card:

```html
<div class="glass-card overflow-x-auto">
<table class="w-full" style="font-size:13px;border-collapse:collapse">
  <thead>
    <tr style="background:#F9FAFB;border-bottom:1px solid #E5E7EB">
      <th style="padding:7px 12px;text-align:left;font-size:10px;font-weight:700;text-transform:uppercase;letter-spacing:0.06em;color:#6B7280">Name</th>
      <th style="padding:7px 12px;text-align:right;font-size:10px;font-weight:700;text-transform:uppercase;letter-spacing:0.06em;color:#6B7280">W</th>
      <th style="padding:7px 12px;text-align:right;font-size:10px;font-weight:700;text-transform:uppercase;letter-spacing:0.06em;color:#6B7280">Actions</th>
    </tr>
  </thead>
  <tbody>
    <tr style="border-bottom:1px solid #F3F4F6;transition:background 0.1s"
        onmouseover="this.style.background='#F9FAFB'" onmouseout="this.style.background=''">
      <td style="padding:9px 12px;font-weight:600;color:#111827">@item.Name</td>
      <td style="padding:9px 12px;text-align:right;color:#6B7280">@item.Role</td>
      <td style="padding:9px 12px;text-align:right">
        <a href="/edit/@item.Id" class="inline-block px-2 py-0.5 rounded text-xs font-medium"
           style="background:#F3F4F6;color:#374151;border:1px solid #E5E7EB;text-decoration:none">Edit</a>
      </td>
    </tr>
  </tbody>
</table>
</div>
```

**Win/loss color conventions in cells:**
```html
<td style="padding:9px 12px;font-weight:700;color:#16A34A">@item.Wins</td>   <!-- wins -->
<td style="padding:9px 12px;font-weight:700;color:#DC2626">@item.Losses</td> <!-- losses -->
<td style="padding:9px 12px;font-size:12px;color:#6B7280">@item.Pct</td>     <!-- percentage/muted -->
```

**Position badge (standings):**
```html
<span style="display:inline-flex;align-items:center;justify-content:center;width:20px;height:20px;border-radius:50%;font-size:11px;font-weight:700;background:#FEF3C7;color:#92400E">1</span>  <!-- gold/1st -->
<span style="display:inline-flex;align-items:center;justify-content:center;width:20px;height:20px;border-radius:50%;font-size:11px;font-weight:700;background:#DBEAFE;color:#1E40AF">2</span>  <!-- blue/2nd -->
<span style="display:inline-flex;align-items:center;justify-content:center;width:20px;height:20px;border-radius:50%;font-size:11px;font-weight:700;background:#F9FAFB;color:#6B7280;border:1px solid #E5E7EB">4</span> <!-- neutral -->
```

---

## Forms

### Field pattern
```html
<div class="mb-4">
  <label class="block text-sm font-semibold mb-1.5" style="color:#111827">
    First Name <span style="color:#DC2626">*</span>
  </label>
  <input asp-for="Input.FirstName" class="w-full rounded px-3 py-2 text-sm"
         style="background:#F9FAFB;color:#111827;border:1px solid #D1D5DB;outline:none;transition:border-color 0.15s" />
  <span asp-validation-for="Input.FirstName" class="text-xs mt-1 block" style="color:#DC2626"></span>
</div>

<!-- Select -->
<div class="mb-4">
  <label class="block text-sm font-semibold mb-1.5" style="color:#111827">Role</label>
  <select asp-for="Input.Role" class="w-full rounded px-3 py-2 text-sm"
          style="background:#F9FAFB;color:#111827;border:1px solid #D1D5DB">
    <option value="">— Select —</option>
  </select>
</div>
```

### Form shell
```html
<form method="post" class="max-w-xl">
  <div asp-validation-summary="ModelOnly" class="mb-4 p-3 rounded text-sm"
       style="background:#FEF2F2;color:#991B1B;border:1px solid #FECACA">
  </div>
  <!-- fields -->
  <div class="flex gap-3 mt-6">
    <button type="submit" class="px-4 py-2 rounded text-sm font-semibold cursor-pointer"
            style="background:#2563EB;color:#fff;border:none">Save</button>
    <a href="..." class="inline-flex items-center px-4 py-2 rounded text-sm font-medium"
       style="background:transparent;color:#111827;border:1px solid #E5E7EB;text-decoration:none">Cancel</a>
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

<!-- Neutral tag -->
<span style="display:inline-block;padding:2px 8px;border-radius:4px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.05em;background:#F3F4F6;color:#374151">General</span>

<!-- Recap tag -->
<span style="background:#DCFCE7;color:#166534;display:inline-block;padding:2px 8px;border-radius:4px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.05em">Recap</span>

<!-- Event tag -->
<span style="background:#FEF3C7;color:#92400E;display:inline-block;padding:2px 8px;border-radius:4px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.05em">Event</span>

<!-- News tag -->
<span style="background:#DBEAFE;color:#1E40AF;display:inline-block;padding:2px 8px;border-radius:4px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.05em">News</span>
```

CSS (in `app.css`):
```css
.badge-win, .label-success {
  display: inline-flex; align-items: center;
  padding: 2px 8px; border-radius: 4px;
  background-color: #DCFCE7; color: #166534;
  font-size: 0.75rem; font-weight: 700;
}
.badge-loss, .label-danger {
  display: inline-flex; align-items: center;
  padding: 2px 8px; border-radius: 4px;
  background-color: #FEE2E2; color: #991B1B;
  font-size: 0.75rem; font-weight: 700;
}
```

---

## Auth Pages

Auth pages use a centered card with white background on the gray page background.

```html
<div class="auth-wrap">
  <div class="auth-card">
    <div class="auth-logo">GTDL</div>
    <h1 class="auth-title">Sign In</h1>
    <p class="auth-sub">Greater Trenton Dart League — member area</p>

    <div class="field">
      <label class="auth-label" asp-for="Email">Email address</label>
      <input asp-for="Email" type="email" class="auth-input" autocomplete="email" />
    </div>
    <div class="field">
      <label class="auth-label" asp-for="Password">Password</label>
      <input asp-for="Password" type="password" class="auth-input" />
    </div>

    <button type="submit" class="auth-btn">Sign in</button>
  </div>
</div>
```

---

## Page Headers

### Public page title
```html
<div class="sec-head" style="margin-bottom:20px">
  <h2 style="font-size:17px;font-weight:700;letter-spacing:-0.02em;color:#111827">Season Standings</h2>
  <a href="..." style="font-size:13px;color:#2563EB;font-weight:500;text-decoration:none">Full table →</a>
</div>
```

### Hero section (homepage)
```html
<section style="padding:56px 0 64px;border-bottom:1px solid #E5E7EB">
  <div style="max-width:1200px;margin:0 auto;padding:0 24px">
    <div style="display:inline-flex;align-items:center;gap:7px;padding:4px 10px;border-radius:100px;background:#EFF6FF;border:1px solid #BFDBFE;font-size:12px;font-weight:600;color:#2563EB;margin-bottom:18px">
      <span style="width:7px;height:7px;border-radius:50%;background:#16A34A;display:inline-block"></span>
      Season 14 · Spring 2025 · Week 13 of 16
    </div>
    <h1 style="font-size:clamp(30px,5vw,54px);font-weight:800;letter-spacing:-0.03em;line-height:1.1;margin-bottom:16px;color:#111827">
      Greater Trenton<br>Dart League
    </h1>
    <p style="font-size:clamp(15px,1.6vw,17px);color:#6B7280;max-width:480px;margin-bottom:32px;line-height:1.65">
      Competitive darts in Trenton, NJ. Weekly match nights, year-round community.
    </p>
    <div style="display:flex;gap:10px;flex-wrap:wrap">
      <a href="#standings" style="display:inline-flex;align-items:center;padding:9px 18px;border-radius:6px;font-size:14px;font-weight:600;background:#2563EB;color:#fff;border:1px solid #2563EB;text-decoration:none">View Standings</a>
      <a href="/Auth/Login" style="display:inline-flex;align-items:center;padding:9px 18px;border-radius:6px;font-size:14px;font-weight:600;background:transparent;color:#111827;border:1px solid #E5E7EB;text-decoration:none">Member Login</a>
    </div>
  </div>
</section>
```

---

## Navigation

Navigation is handled by `_DefaultNav.cshtml` (public) and `_ManageNav.cshtml` (admin). Both are sticky with light background and `backdrop-filter: blur(14px)`.

**Public nav** is a single bar with inline links on desktop and hamburger + mobile dropdown on mobile. No fixed bottom bar.

Do not inline nav HTML in pages — always use the partials.

---

## Scoresheet / Match Display

Defined in `app.css`. Updated to light theme.

```html
<div id="scoresheet">
  <div class="scoresheet-row header">
    <div class="scoresheet-col-team">Home Team</div>
    <div class="scoresheet-col-score">3</div>
    <div class="scoresheet-col-score">2</div>
    <div class="scoresheet-col-team" style="text-align:right">Away Team</div>
  </div>
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

Sprite sheet — positions defined in `app.css` as `.award-img.*` classes. Unchanged.

```html
<div class="award-img high-on" title="High On"></div>
<div class="award-img high-out" title="High Out"></div>
<div class="award-img round-9" title="9-Dart Round"></div>
<div class="award-img t-71" title="Ton 71"></div>
```

---

## Pagination / Week Navigation

```html
<div class="flex items-center gap-2 mb-6">
  <a href="?week=prev" style="padding:6px 12px;font-size:13px;border-radius:6px;background:transparent;border:1px solid #E5E7EB;color:#111827;text-decoration:none">
    &laquo; Prev
  </a>
  <span style="font-size:13px;color:#6B7280;flex:1;text-align:center">
    <span style="font-weight:700;color:#111827">Week 3 of 14</span>
    <span style="margin:0 4px;color:#9CA3AF">—</span>
    January 22, 2026
  </span>
  <a href="?week=next" style="padding:6px 12px;font-size:13px;border-radius:6px;background:transparent;border:1px solid #E5E7EB;color:#111827;text-decoration:none">
    Next &raquo;
  </a>
</div>
```

---

## Icons

Font Awesome 6.5.0 loaded from cdnjs. Use `fa-solid` for solid style.

Common icons in use:
```html
<i class="fa-solid fa-ranking-star" style="color:#2563EB"></i>   <!-- standings -->
<i class="fa-solid fa-calendar-days" style="color:#2563EB"></i>  <!-- schedule -->
<i class="fa-solid fa-chart-bar" style="color:#2563EB"></i>      <!-- stats -->
<i class="fa-solid fa-trophy" style="color:#2563EB"></i>         <!-- awards -->
<i class="fa-brands fa-facebook" style="color:#2563EB"></i>      <!-- social -->
<i class="fa-solid fa-check fa-lg"></i>                          <!-- success -->
<i class="fa-solid fa-xmark fa-lg"></i>                          <!-- error -->
```

Icon color in nav/accent contexts: `#2563EB` (blue). In body text, inherit from parent.

---

## Footer

Three-column footer grid:

```html
<footer style="border-top:1px solid #E5E7EB;padding:44px 0 28px">
  <div style="max-width:1200px;margin:0 auto;padding:0 24px;display:grid;gap:24px;grid-template-columns:2fr 1fr 1fr">
    <div>
      <span style="font-size:22px;font-weight:800;letter-spacing:-0.04em;color:#2563EB">GTDL</span>
      <p style="font-size:13px;color:#6B7280;line-height:1.65;margin-top:10px;max-width:260px">
        Greater Trenton Dart League — serving the Trenton, NJ darts community since 2008.
      </p>
    </div>
    <div>
      <h4 style="font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.07em;color:#6B7280;margin-bottom:12px">League</h4>
      <ul style="list-style:none;display:flex;flex-direction:column;gap:8px;padding:0;margin:0">
        <li><a href="#" style="font-size:14px;color:#111827;text-decoration:none">Standings</a></li>
      </ul>
    </div>
    <div>
      <h4 style="font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.07em;color:#6B7280;margin-bottom:12px">Members</h4>
      <ul style="list-style:none;display:flex;flex-direction:column;gap:8px;padding:0;margin:0">
        <li><a href="/Auth/Login" style="font-size:14px;color:#111827;text-decoration:none">Login</a></li>
      </ul>
    </div>
  </div>
  <div style="max-width:1200px;margin:28px auto 0;padding:20px 24px 0;border-top:1px solid #E5E7EB;display:flex;justify-content:space-between;flex-wrap:wrap;gap:8px">
    <p style="font-size:12px;color:#6B7280">&copy; 2025 Greater Trenton Dart League. All rights reserved.</p>
    <p style="font-size:12px;color:#6B7280">Trenton, NJ</p>
  </div>
</footer>
```

---

## Responsive Conventions

- **Mobile-first.** All Tailwind classes apply at mobile; prefix with `sm:` (640px) or `md:` (768px) for larger screens.
- **No fixed bottom nav.** Public pages have no mobile bottom tab bar in the new design.
- **Nav hidden/shown:** `hidden md:flex` (desktop only), `flex md:hidden` (mobile only).
- **Typography scale:** `clamp(30px, 5vw, 54px)` for hero h1.
- **Main content max-width:** `1200px` / `max-w-7xl`.

---

## Dos and Don'ts

**Do:**
- Use white card backgrounds with `1px solid #E5E7EB` borders
- Use `#2563EB` blue for all primary actions, links, and accent elements
- Use system font stack — no web font import needed
- Keep `#DC2626` red for losses/danger and `#16A34A` green for wins — these are semantic
- Use `#F9FAFB` for alternating table rows and input backgrounds

**Don't:**
- Use dark or black backgrounds on public pages
- Use `rgba(255,255,255,...)` for borders or surface colors (those were dark-theme conventions)
- Use `backdrop-filter: blur()` on cards — the new design has no blur effects
- Use Rajdhani or Inter as Google Fonts (removed from layout)
- Add box shadows to cards — thin borders are the depth signal in this design
