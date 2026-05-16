# UI Design Reference

Greater Trenton Dart League uses the dark product-app direction from `new-design/`: compact, sharp, charcoal, amber, and data-first. The current app is Razor Pages + Tailwind, not React/MUI. When thinking in MUI terms, treat these as the theme tokens and component variants to translate into Razor/Tailwind markup.

## Source Samples

- `new-design/gtdl-index-dark.html`: public homepage, sticky top nav, hero with bridge photo, compact bulletin/content cards, standings and schedule widgets.
- `new-design/member-dashboard.html`: authenticated dashboard shell, login card, stat cards, panels, tabs, dense list items.
- `new-design/post-detail.html`: content detail styling for posts/articles.

## Design Direction

Dark charcoal league operations UI, not a parchment sports-club theme. The memorable signal is the amber bridge accent on a near-black product shell, with real GTDL logo/photo assets and compact information panels.

Use:
- System font stack for a modern app feel.
- Amber for primary actions and active navigation.
- Muted gray for secondary text.
- Green/red only for semantic results.
- Squared practical radii: 6px, 10px, 16px.
- Tables and panels that feel dense, scannable, and stable.

Avoid:
- Cream-heavy typography.
- Rajdhani display headings.
- Large decorative headers on every page.
- Soft glassmorphism, oversized rounded cards, or marketing-style section cards.
- Purple/blue gradients or unrelated illustration.

## MUI Theme Translation

If this were MUI, use this mental model:

```ts
const theme = createTheme({
  palette: {
    mode: "dark",
    background: {
      default: "oklch(10% 0.014 240)",
      paper: "oklch(14% 0.012 240)"
    },
    text: {
      primary: "oklch(91% 0.005 240)",
      secondary: "oklch(56% 0.008 240)"
    },
    primary: {
      main: "oklch(75% 0.17 72)",
      dark: "oklch(65% 0.15 72)",
      contrastText: "oklch(10% 0.014 240)"
    },
    success: { main: "oklch(56% 0.14 145)" },
    error: { main: "oklch(54% 0.19 28)" },
    divider: "oklch(23% 0.010 240)"
  },
  shape: { borderRadius: 6 },
  typography: {
    fontFamily: "-apple-system, BlinkMacSystemFont, 'SF Pro Text', 'Segoe UI', system-ui, sans-serif",
    h1: { fontWeight: 800, letterSpacing: "-0.03em" },
    h2: { fontWeight: 800, letterSpacing: "-0.03em" },
    button: { fontWeight: 700, textTransform: "none" }
  }
});
```

MUI component equivalents:

| Pattern | MUI equivalent | Razor/Tailwind implementation |
|---|---|---|
| Sticky site nav | `AppBar` + `Toolbar` | `_DefaultNav.cshtml`, 56px height, blur background, logo left |
| Admin nav | `AppBar` + grouped menus | `_ManageNav.cshtml`, same charcoal shell |
| Main page width | `Container maxWidth="lg"` | `.site-shell`, max 1200px, 24px desktop padding |
| Homepage hero | `Box` with image background | `.site-hero`, bridge photo, dark overlay, logo card |
| Data panel | `Paper variant="outlined"` | `.glass-card`, `--color-surface`, `--color-border`, 10px radius |
| Panel header | `CardHeader` | `.panel-head`, 14px/18px padding, border bottom |
| Primary button | `Button variant="contained"` | `.btn-primary`, amber fill, dark text |
| Secondary button | `Button variant="outlined"` | `.btn-ghost`, transparent with border |
| Badges/chips | `Chip size="small"` | compact rounded spans, 11px uppercase labels |
| Tables | `Table size="small"` | compact rows, muted uppercase headers, tabular numeric data |

## CSS Tokens

Defined in `TrentonDarts.Web/ClientApp/app.css`:

```css
:root {
  --color-bg:        oklch(10% 0.014 240);
  --color-surface:   oklch(14% 0.012 240);
  --color-surface-hi:oklch(19% 0.010 240);
  --color-surface-2: oklch(17% 0.011 240);
  --color-border:    oklch(23% 0.010 240);
  --color-text:      oklch(91% 0.005 240);
  --color-muted:     oklch(56% 0.008 240);
  --color-red:       oklch(54% 0.19 28);
  --color-green:     oklch(56% 0.14 145);
  --color-gold:      oklch(75% 0.17 72);
  --color-gold-hover:oklch(65% 0.15 72);
  --color-gold-bg:   oklch(18% 0.07 72);
  --radius-sm: 6px;
  --radius-md: 10px;
  --radius-lg: 16px;
  --max-w: 1200px;
}
```

## Layout

Public pages:

```html
<body class="min-h-screen flex flex-col">
  <div id="site-nav">...</div>
  <main class="flex-1 site-shell">...</main>
</body>
```

Homepage hero is the only large first-viewport brand treatment:

```html
<section class="site-hero">
  <div class="site-hero-inner">
    <div>
      <span class="site-pill">League night is live</span>
      <h1 class="site-hero-title">Greater Trenton Dart League</h1>
      <p class="site-hero-copy">...</p>
    </div>
    <div class="site-hero-logo"><img src="/images/trentondarts-logo.png"></div>
  </div>
</section>
```

Interior pages should start directly with content. Do not reintroduce a full hero/header on every page.

## Components

### Panels

```html
<section class="glass-card">
  <div class="panel-head">
    <h2 class="panel-title">Schedule</h2>
    <a class="text-xs font-medium" href="...">Full</a>
  </div>
  <div class="p-4">...</div>
</section>
```

### Buttons

Use `.btn-primary` for primary actions and `.btn-ghost` for secondary actions. Keep labels short and include Font Awesome icons when the action benefits from quick recognition.

```html
<a class="btn-primary" href="/season/1"><i class="fa-solid fa-ranking-star"></i>Standings</a>
<a class="btn-ghost" href="/season/1/schedule"><i class="fa-solid fa-calendar-days"></i>Schedule</a>
```

### Tables

Tables should be compact and numeric. Use muted uppercase headers, 1px dividers, and no zebra striping unless rows are interactive.

```html
<div class="glass-card overflow-x-auto">
  <table class="w-full text-sm">
    <thead>
      <tr style="border-bottom:1px solid var(--color-border);color:var(--color-muted)">
        <th class="px-3 py-2 text-left text-xs font-bold uppercase tracking-wide">Team</th>
      </tr>
    </thead>
  </table>
</div>
```

### Forms

Inputs mirror MUI `outlined` dark fields:

- Background: `var(--color-surface)`.
- Border: `var(--color-border)`.
- Focus ring: amber at low opacity.
- Label: 13px, 600 weight, primary or muted text depending on density.

### Mobile

Top nav collapses into the hamburger menu. The bottom mobile quick nav may remain for current-season shortcuts, but it must use the same charcoal surface and muted/amber icon colors. Keep fixed navigation from covering content by preserving the mobile bottom padding in `.site-shell`.
