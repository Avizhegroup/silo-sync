# Silo UI Design System — Visual Design & Color Palette Guide

## System Role
You are a senior front-end architect working on the **Silo WMS** Blazor application.
When designing or restyling components, you prioritize **visual consistency, color palette adherence, and design system principles** above implementation details.

---

## Design Philosophy

The Silo design system embodies a **modern, clean WMS dashboard aesthetic** with these core principles:

### Visual Hierarchy
- **Clarity over decoration** — minimal ornamental elements, focus on content and data
- **Purposeful color usage** — each color carries semantic meaning (brand, status, interaction state)
- **Consistent depth system** — subtle shadows create layering without clutter
- **Icon-first interactions** — Material Icons provide universal, language-agnostic affordances

### Layout Principles
- **Card-based composition** — all content surfaces are cards (`border-0 shadow-sm`)
- **Flexible spacing** — Bootstrap 5 gap utilities (`gap-2`, `gap-3`) over fixed margins
- **RTL-ready** — designed for Persian UI; all layouts support RTL text direction
- **Responsive grid** — Bootstrap's 12-column grid ensures mobile-to-desktop scalability

### Interaction Design
- **Subtle state changes** — hover/active states use background color shifts, not borders
- **Smooth transitions** — `transition: .15s–.25s` on interactive elements
- **Clear affordances** — buttons, pills, and nav items have distinct resting and active states
- **Loading states** — `TelerikLoaderContainer` provides consistent async feedback

---

## Color Palette & Usage

The design system uses a **two-tier color architecture**: **primitive colors** (base palette) and **semantic tokens** (usage-specific aliases).

> **Golden Rule**: Always use **semantic tokens** in component styles. Primitive colors are for token definition only.

---

### Primitive Palette (Foundation Layer)

These are the raw color values. **Do not reference these directly in component CSS.**

#### Neutrals (Greys)
| Variable | Hex | Visual Role |
|---|---|---|
| `$color-white` | `#ffffff` | Pure white |
| `$color-black` | `#000000` | Pure black |
| `$color-grey-100` | `#f3f3f3` | Lightest surface tint |
| `$color-grey-200` | `#f2f2f2` | Panel / card background (legacy) |
| `$color-grey-300` | `#e8e8e8` | Hover state background |
| `$color-grey-400` | `#d0d0d0` | Borders, dividers |
| `$color-grey-500` | `#b8b8b8` | Disabled element tint |
| `$color-grey-600` | `#707070` | Muted / secondary text |
| `$color-grey-700` | `#3c4043` | Dark label text, icons |

#### Brand Colors
| Variable | Hex | Visual Role |
|---|---|---|
| **Teal (Primary Brand)** |
| `$color-teal` | `#97DCE1` | Primary brand accent — borders, pills, highlights |
| `$color-teal-alpha` | `#97DCE120` | Translucent brand tint |
| **Blues (Action & Links)** |
| `$color-blue-light` | `#0CCAEA` | Profile name highlight, bright accent |
| `$color-blue-primary` | `#1a73e8` | Active state, links (Google Material blue) |
| `$color-blue-navy` | `#003e81` | Primary action buttons, table headers |
| **Purple (Secondary)** |
| `$color-purple` | `#7E6CC0` | Secondary actions, nav links, logout button |

#### Status / Semantic Colors
| Variable | Hex | Visual Role |
|---|---|---|
| `$color-red` | `#B00020` | Error, danger, deletion |
| `$color-green` | `#28a745` | Success, Excel export button |
| `$color-green-light` | `#5AEC4F` | Lime / live indicator |
| `$color-orange` | `#fd7e14` | Chart accent (line charts) |
| `$color-orange-button` | `#f36b16` | PDF export button |
| `$color-yellow` | `#FFCC00` | Warning, caution |

---

### Semantic Tokens (Usage Layer)

These tokens map to specific UI elements. **Always use these in your SCSS/CSS.**

#### Surfaces
| Token | Value | Usage |
|---|---|---|
| `$surface-page` | `#f5f5f5` | Body background |
| `$surface-card` | `$color-white` | Card backgrounds |
| `$surface-panel` | `$color-grey-200` | Panels, dropdown menus |
| `$surface-sidebar` | `#f8f9fa` | Sidebar / category panel background |

#### Borders
| Token | Value | Usage |
|---|---|---|
| `$border-color` | `#e8eaed` | Navbar, card outlines, dividers (modern) |
| `$border-color-legacy` | `$color-grey-400` | Older component borders |
| `$border-color-strong` | `$color-black` | Strong dividers (rarely used) |

#### Text
| Token | Value | Usage |
|---|---|---|
| `$text-primary` | `#202124` | Body text, primary labels |
| `$text-secondary` | `$color-grey-600` | Secondary labels, KPI captions |
| `$text-muted` | `#80868b` | Placeholder text, subtle hints |
| `$text-on-dark` | `$color-white` | Text on dark backgrounds (e.g., table headers) |
| `$text-link` | `#0366d6` | Hyperlinks |
| `$text-nav-link` | `$color-purple` | Sidebar navigation items |

#### Interactive States
| Token | Value | Usage |
|---|---|---|
| **Primary Action** | | |
| `$action-primary-bg` | `$color-blue-navy` | Primary buttons, table headers (`thead`) |
| `$action-primary-text` | `$color-white` | Text on primary buttons |
| **Brand Accent** | | |
| `$action-accent-bg` | `$color-teal` | Accent buttons, brand pills |
| `$action-accent-text` | `$color-white` | Text on accent buttons |
| **Active / Selected** | | |
| `$action-active-bg` | `$color-blue-primary` | Active nav items, selected state |
| `$action-active-text` | `$color-white` | Text on active elements |

#### Navbar
| Token | Value | Usage |
|---|---|---|
| `$navbar-height` | `56px` | Fixed navbar height |
| `$navbar-bg` | `$color-white` | Navbar background |
| `$navbar-border` | `$border-color` | Navbar bottom border |
| `$navbar-icon-hover-bg` | `#f1f3f4` | Icon button hover background |

#### Pills & Badges
| Token | Value | Usage |
|---|---|---|
| `$pill-date-bg` | `#e6f4ea` | Date pill background (green tint) |
| `$pill-date-text` | `#1e7e34` | Date pill text |
| `$pill-version-bg` | `#e8f0fe` | Version pill background (blue tint) |
| `$pill-version-text` | `$color-blue-primary` | Version pill text |

#### KPI Cards
| Token | Value | Usage |
|---|---|---|
| `$kpi-icon-size` | `52px` | Icon circle diameter |
| `$kpi-label-size` | `.8rem` | KPI label font size |

#### Tables & Grids
| Token | Value | Usage |
|---|---|---|
| `$thead-bg` | `$color-blue-navy` | Table header background |
| `$thead-text` | `$color-white` | Table header text |
| `$tbody-row-bg` | `#f5fdff` | Table row subtle tint |
| `$tbody-row-alt-bg` | `#c8e1fd` | Alternate row / action bar background |

#### Overlays
| Token | Value | Usage |
|---|---|---|
| `$backdrop-bg` | `rgba(0,0,0,.35)` | Modal / menu backdrop |

---

## Color Application Patterns

### 1. Button Color Semantics
| Button Type | Background | Text | Icon | Usage |
|---|---|---|---|---|
| **Primary** | `$action-primary-bg` (navy) | `$action-primary-text` (white) | White | Main CTA, submit, save |
| **Accent** | `$action-accent-bg` (teal) | `$action-accent-text` (white) | White | Brand-aligned actions |
| **Warning** | Bootstrap `btn-warning` | Dark grey | Dark | Reset, clear filters |
| **Danger** | Bootstrap `btn-danger` | White | White | Delete, reject |
| **Success** | `$color-green` | White | White | Confirm, approve |

### 2. KPI Icon Color Mapping
Use Bootstrap semantic background tints:
- `bg-primary-subtle` / `text-primary` — general metrics
- `bg-success-subtle` / `text-success` — positive metrics (revenue, completed tasks)
- `bg-warning-subtle` / `text-warning` — caution metrics (low stock, pending)
- `bg-danger-subtle` / `text-danger` — critical metrics (errors, overdue)

### 3. Status Indicator Colors
| Status | Color | Usage |
|---|---|---|
| Active / Live | `$color-green-light` (`#5AEC4F`) | Real-time indicators, live sessions |
| Success | `$color-green` (`#28a745`) | Completed actions, successful operations |
| Warning | `$color-yellow` (`#FFCC00`) | Low priority alerts, warnings |
| Error | `$color-red` (`#B00020`) | Failures, validation errors |
| Info / Selected | `$color-blue-primary` (`#1a73e8`) | Active nav items, selected rows |

### 4. Chart Color Usage
- **Primary series**: `$color-blue-primary` (`#1a73e8`)
- **Secondary series**: `$color-teal` (`#97DCE1`)
- **Tertiary series**: `$color-purple` (`#7E6CC0`)
- **Accent series**: `$color-orange` (`#fd7e14`)
- **Negative/error**: `$color-red` (`#B00020`)
- **Positive**: `$color-green` (`#28a745`)

---

## Typography System

### Font Family
- **Primary**: `IRANSans` (loaded in `abstracts/font.scss`)
- **Applied globally** to `body, h1–h6, input, textarea`

### Font Weights
| Weight | Numeric | Usage |
|---|---|---|
| Ultra Light | 200 | Rarely used |
| Light | 300 | Subtle labels |
| Regular | 400 | Body text, inputs |
| Medium | 500 | Emphasized text |
| Bold | 700 | Card headers, KPI labels |
| Black | 900 | Extra emphasis (rare) |

### Font Sizes
| Element | Size | Bootstrap Class | Usage |
|---|---|---|---|
| **Base** | `1rem` (16px) | — | Default text |
| **Small** | `.875rem` (14px) | `.small` | Helper text |
| **Tiny** | `.72rem–.82rem` | Custom | Pills, nav items, captions |
| **KPI Labels** | `.8rem` | `$kpi-label-size` | KPI card labels |
| **Card Headers** | `1rem` | `.fw-semibold` | Chart card titles |
| **KPI Values** | Responsive | `.fw-bold .fs-4` | Large metric numbers |

### Line Height
- **Default**: `1.5` (Bootstrap default)
- **KPI Values**: `1.2` (tight for large numbers)
- **Pills**: `1.4` (vertically centered in badge)

---

## Spacing & Layout

### Spacing Scale (Bootstrap 5)
| Class | Value | Usage |
|---|---|---|
| `gap-1` | `.25rem` (4px) | Tight icon + text pairs |
| `gap-2` | `.5rem` (8px) | Filter bar elements |
| `gap-3` | `1rem` (16px) | Card rows, default row gutters |
| `gap-4` | `1.5rem` (24px) | Section spacing |
| `mb-3` | `1rem` | Margin-bottom for filter bars |
| `mb-4` | `1.5rem` | Margin-bottom for chart rows |
| `py-2` | `.5rem` top/bottom | Tight card bodies (filter bars, quick-access) |
| `p-2` | `.5rem` all sides | Chart card bodies |

### Grid & Cards
- **Page wrapper**: `.dashboard-container` with `padding: 1.25rem` (20px)
- **Row gutters**: `row g-3` (16px gaps between columns)
- **Card spacing**: `border-0 shadow-sm` (no border, subtle shadow)
- **KPI columns**: `col-md-4` (3 per row on desktop)
- **Chart columns**: `col-md-6` (2 per row) or `col-md-12` (full width)

### Depth System (Shadows)
| Level | Box Shadow | Usage |
|---|---|---|
| **Card** | `0 .25rem .75rem rgba(0,0,0,.05)` | Default card shadow (`.shadow-sm`) |
| **Card Hover** | `0 4px 18px rgba(0,0,0,.12)` | KPI card hover effect |
| **Elevated** | `0 8px 28px rgba(0,0,0,.14)` | Profile panel, dropdowns |
| **Navbar** | `0 1px 6px rgba(0,0,0,.06)` | Navbar subtle elevation |
| **Mega Menu** | `0 6px 24px rgba(0,0,0,.10)` | Drawer / mega-menu shadow |

---

## Icon System

### Icon Source
- **Library**: Material Icons (via `MaterialIcon` component)
- **Reference**: `MaterialIconsHelper.{IconName}` constants

### Icon Sizing
| Context | Size | CSS |
|---|---|---|
| **KPI Icons** | `1.5rem` (24px) | Inside `52px` circle |
| **Navbar Icons** | `1.25rem` (20px) | Inside `36px` button |
| **Pill Icons** | `.9rem` (14.4px) | Date/version pills |
| **Button Icons** | `1rem–1.25rem` | Filter bar, action buttons |

### Icon Color Mapping
| Context | Color | Token |
|---|---|---|
| **Navbar Icons (default)** | `#5f6368` | `$color-grey-700` |
| **Navbar Icons (hover)** | `#202124` | `$text-primary` |
| **Navbar Icons (active)** | `#1a73e8` | `$color-blue-primary` |
| **KPI Icons** | Inherits parent `text-{color}` | Bootstrap semantic colors |
| **Brand Icons** | `#97DCE1` | `$color-teal` |
| **Menu Category Icons** | `#97DCE1` (default), `#1a73e8` (active) | Brand / active blue |

---

## Component-Specific Design Specs

### 1. KPI Card
**Visual Anatomy:**
```
┌─────────────────────────────────────┐
│ ┌──────┐                            │
│ │ Icon │  Label (0.8rem, grey-600)  │
│ │ 52px │  Value (fw-bold, fs-4)     │
│ └──────┘                            │
└─────────────────────────────────────┘
```
- **Background**: `$surface-card` (white)
- **Border**: None (`border-0`)
- **Shadow**: `.shadow-sm` (default), `0 4px 18px rgba(0,0,0,.12)` on hover
- **Border-radius**: `12px`
- **Icon circle**: `52px` diameter, `bg-{color}-subtle`, centered Material Icon
- **Gap**: `gap-3` (16px) between icon and text
- **Transition**: `box-shadow .2s`

### 2. Chart Card
**Visual Anatomy:**
```
┌───────────────────────────────┐
│ Header (bg-transparent, fw-semibold)  │
├───────────────────────────────┤
│                               │
│     TelerikChart              │
│     (p-2 card-body)           │
│                               │
└───────────────────────────────┘
```
- **Header background**: `transparent`
- **Header text**: `.fw-semibold`, `$text-primary`
- **Header border**: `1px solid $border-color` (bottom only)
- **Body padding**: `p-2` (8px all sides)
- **Chart width**: Always `100%`
- **Chart height**: Explicit `{N}px` (never `auto`)
- **Card stretch**: `h-100` to match row height

### 3. Navbar
**Visual Anatomy:**
```
┌─────────────────────────────────────────────────────────────┐
│ [menu-btn] [logo] │ [breadcrumb ──────────] │ [pills][actions] │
│     silo-nav-start  │  silo-nav-breadcrumb  │  silo-nav-end   │
└─────────────────────────────────────────────────────────────┘
```
- **Height**: `56px` (`$navbar-height`)
- **Background**: `$navbar-bg` (white)
- **Border**: `1px solid $navbar-border` (bottom only)
- **Shadow**: `0 1px 6px rgba(0,0,0,.06)`
- **Zone gaps**: `.25rem` (start), `1rem` (padding inside breadcrumb), `.35rem` (end)

**Icon Buttons (`nav-icon-btn`):**
- **Size**: `36px` circle
- **Background**: `transparent` (default), `#f1f3f4` (hover), `#e8f0fe` (active)
- **Icon color**: `#5f6368` (default), `#202124` (hover), `#1a73e8` (active)
- **Transition**: `background .15s, color .15s`

**Pills (`nav-pill`):**
- **Padding**: `.2rem .65rem`
- **Border-radius**: `20px`
- **Font-size**: `.72rem`
- **Font-weight**: `600`
- **Date pill**: `background: #e6f4ea; color: #1e7e34`
- **Version pill**: `background: #e8f0fe; color: #1a73e8`

### 4. Filter Bar
**Visual Anatomy:**
```
┌─────────────────────────────────────────────────────┐
│ Label | Input | Input | [Refresh btn] [Search btn] │
│ (d-flex flex-row align-items-center gap-2)         │
└─────────────────────────────────────────────────────┘
```
- **Card body padding**: `py-2` (8px top/bottom)
- **Label**: `mb-0` (no margin to stay aligned)
- **Reset button**: `btn-warning` + `MaterialIconsHelper.Refresh`
- **Search button**: `btn-primary` + `MaterialIconsHelper.Search`
- **Inputs**: `class="form-control input-search-span"`

### 5. Pills & Badges
**Date Pill:**
- Background: `#e6f4ea` (light green)
- Text: `#1e7e34` (dark green)
- Icon: `MaterialIconsHelper.Today`

**Version Pill:**
- Background: `#e8f0fe` (light blue)
- Text: `#1a73e8` (primary blue)
- Icon: `MaterialIconsHelper.Info`

**Logout Button (pill-shaped):**
- Background: `#7E6CC0` (purple)
- Text: White
- Border-radius: `20px`
- Icon: `MaterialIconsHelper.Logout`

---

## Animation & Transitions

### Transition Timings
| Context | Duration | Easing | Usage |
|---|---|---|---|
| **Hover states** | `.15s` | Linear | Icon buttons, nav links |
| **Card shadow** | `.2s` | Linear | KPI card hover |
| **Menu fade-in** | `.18s` | Ease | Mega-menu drawer |
| **Button transforms** | `.25s` | Ease | View toggle buttons |

### Keyframe Animations
**Navbar Menu Fade-In:**
```scss
@keyframes navFadeIn {
    from { opacity: 0; transform: translateY(-6px); }
    to   { opacity: 1; transform: translateY(0); }
}
```
- Applied to: `.silo-menu-drawer`, `.nav-profile-panel`
- Duration: `.15s–.18s`

**Loading Spinner:**
```scss
@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}
```
- Applied to: `.loading-spinner`
- Duration: `1s linear infinite`

---

## SCSS Architecture & File Organization

### Compilation Pipeline
```
Site.scss (entry point)
  ├── abstracts/font.scss       ← @font-face declarations
  ├── abstracts/colors.scss     ← All color tokens (EDIT HERE for colors)
  ├── abstracts/measures.scss   ← Breakpoints, spacing variables
  ├── libraries/bootstrap.min.css
  ├── libraries/bootstrap.rtl.css
  └── pages/pages.scss
       ├── reports/inventoryconflicts.scss
       ├── account/login.scss
       ├── truck/trucklogin.scss
       ├── truck/index.scss
       ├── gate/truckCrossReport.scss
       ├── warehouse/image-analysis.scss
       └── general.scss          ← Global helpers (buttons, cards, utilities)
```

### Where to Add New Styles
| Scenario | Target File |
|---|---|
| **New color / token** | `abstracts/colors.scss` |
| **New spacing / breakpoint** | `abstracts/measures.scss` |
| **Global utility class** | `pages/general.scss` |
| **New domain page** | Create `pages/{domain}/{pageName}.scss` + `@import` in `pages/pages.scss` |
| **Existing domain page** | Matching partial (e.g., `pages/reports/…`) |
| **Shared component** | Create `pages/shared/{component}.scss` + `@import` in `pages/pages.scss` |

### SCSS Nesting Rules
- ✅ Always reference **semantic tokens** from `colors.scss` — **never raw hex literals**
- ✅ Use `&` parent selector for modifiers (`.kpi-card { &:hover { … } }`)
- ✅ Nest child elements, pseudo-classes, and states
- ✅ Wrap page-scoped styles in `.page-{name} { … }`
- ❌ Avoid nesting deeper than **3 levels** (keeps specificity low)

---

## Design System Quick Reference

### Do's ✅
- Use **semantic color tokens** (`$action-primary-bg`, `$text-secondary`, etc.)
- Use **Bootstrap 5 utility classes** (`d-flex`, `gap-3`, `fw-semibold`, `shadow-sm`)
- Use **Material Icons** for all iconography (`MaterialIconsHelper.*`)
- Apply **subtle shadows** for depth (`box-shadow: 0 4px 18px rgba(0,0,0,.12)`)
- Use **card-based layout** with `border-0 shadow-sm`
- Use **pills for badges** (rounded `20px`, tight padding, colored backgrounds)
- Use **RTL-friendly layout** (avoid `margin-left`, use Bootstrap `gap-*`)

### Don'ts ❌
- ❌ Write raw hex colors in component CSS (use tokens)
- ❌ Use inline `style=""` attributes (use SCSS or utility classes)
- ❌ Use `<table>` for layout (use flex/grid)
- ❌ Use `position: absolute` with px offsets for responsive layout
- ❌ Use `<img>` for interactive buttons (wrap in `.nav-icon-btn`)
- ❌ Use colored card headers (`bg-primary`, etc.) — use `bg-transparent fw-semibold`
- ❌ Use `<RowTemplate>` in `TelerikGrid` (breaks column sizing)
- ❌ Use heavy borders or gradients (keep design flat and clean)

---

## Loading States & Feedback

### TelerikLoaderContainer — the only loading pattern

> ❌ **Never use custom spinners, CSS animations, or any other loading approach.**
> `TelerikLoaderContainer` is the single, canonical loading indicator for all pages and components in this application.

**Usage:**
```razor
<TelerikLoaderContainer Visible="IsLoading"
                        LoaderPosition="@LoaderPosition.End"
                        LoaderType="LoaderType.InfiniteSpinner"
                        Text="@TextResources.APP_StringKeys_Loading" />
```

**Rules:**
- Always placed as the **last element** in the `.razor` file
- `IsLoading` is a `bool` field on the code-behind, set to `true` before async work and `false` when data is ready
- Always include `Text` with the localized loading string
- `LoaderPosition` and `LoaderType` are fixed — do not vary them per page

---

## Sample Component Patterns

### Pattern: KPI Row (3 Cards)
```razor
<div class="row g-3 mb-4">
    <div class="col-md-4"><!-- KPI Card 1 --></div>
    <div class="col-md-4"><!-- KPI Card 2 --></div>
    <div class="col-md-4"><!-- KPI Card 3 --></div>
</div>
```

### Pattern: Chart Row (2 Cards)
```razor
<div class="row g-3 mb-4">
    <div class="col-md-6"><div class="card shadow-sm border-0 h-100">...</div></div>
    <div class="col-md-6"><div class="card shadow-sm border-0 h-100">...</div></div>
</div>
```

### Pattern: Full-Width Detail Grid
```razor
<div class="row g-3 mb-4">
    <div class="col-12">
        <div class="card shadow-sm border-0">
            <div class="card-header bg-transparent fw-semibold">Grid Title</div>
            <div class="card-body p-0">
                <TelerikGrid ...>...</TelerikGrid>
            </div>
        </div>
    </div>
</div>
```

---

## RTL & Accessibility Considerations

### RTL (Right-to-Left) Support
- ✅ Use `gap-*` instead of `margin-left` / `margin-right`
- ✅ Use `flex-direction: row-reverse` where needed
- ✅ Bootstrap RTL CSS is imported (`libraries/bootstrap.rtl.css`)
- ✅ All text fields support RTL via `direction: rtl` CSS

### Accessibility
- ✅ All icon buttons have `title` attributes
- ✅ Color contrast meets WCAG AA (dark text on light surfaces)
- ✅ Keyboard navigation supported in grids (`Navigable="true"`)
- ✅ Loading states announce via `Text` prop on `TelerikLoaderContainer`

---

## Checklist for Designing a New Component

- [ ] Selected **semantic color tokens** from `abstracts/colors.scss`
- [ ] Applied **card-based surface** (`border-0 shadow-sm`)
- [ ] Used **Material Icons** for all interactive elements
- [ ] Applied **appropriate shadow** for depth (`.shadow-sm` or custom)
- [ ] Used **Bootstrap utilities** for spacing (`gap-*`, `mb-*`, `py-*`)
- [ ] Defined **hover/active states** with subtle color transitions
- [ ] Ensured **RTL compatibility** (no fixed left/right margins)
- [ ] Added **loading state** if async (`TelerikLoaderContainer`)
- [ ] Tested **responsive behavior** (mobile, tablet, desktop)
- [ ] Verified **color contrast** for accessibility

---

This guide prioritizes **visual design, color usage, and design system principles**. For implementation details (SCSS compilation, file structure, Blazor component patterns), refer to the supplementary technical documentation.
