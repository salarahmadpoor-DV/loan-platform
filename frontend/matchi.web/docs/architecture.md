# Frontend architecture (matchi.web)

Independent React application at `frontend/matchi.web`. It is **not** part of `dotnet build`. At runtime it talks to Matchi HTTP APIs only (`VITE_API_BASE_URL`).

## Stack (architecture v2)

| Layer | Choice |
|---|---|
| UI | React 19 + TypeScript + Vite |
| Components | Material UI 6, `direction: rtl`, `faIR` locale |
| Server state | TanStack Query 5 |
| Session | Zustand (access token in memory + `localStorage`) |
| HTTP | Axios (`shared/api/httpClient.ts`) |
| Routing | React Router 7 |
| Copy | `t(key)` catalogs, default **fa-IR** RTL |

There is **no Seller** workspace or role. Marketplace parties are Customer, independent Provider, and Business (`BusinessProvider` is membership only).

Request kinds are **Service | Product | Hybrid**. UI must not assume Service-only.

## Layers

```text
app/          bootstrap: theme, QueryClient, router, providers
layouts/      Public / Customer / Provider / Business shells
features/     product UI by workspace + domain
shared/       api, auth, i18n, navigation, ui primitives, marketplace types
```

## Customer flow implemented (Tasks 9.1–9.9)

```text
OTP login
  → Customer dashboard
  → Create / list / detail Request (Service | Product | Hybrid)
  → Matching (read-only)
  → Proposals (list / detail / accept)
  → Deal list / detail
  → Execution + assignments (read-only)
  → Review (eligibility + XOR submit)
```

Provider foundation (Task 11.1):

```text
OTP login (JWT role PROVIDER)
  → /provider/dashboard
  → inbox / my proposals / my deals / my executions / profile (lists only)
```

Provider routes include inbox lists (Task 11.1–11.2) and Provider create-proposal (Task 11.3). The Provider workspace uses the same Matchi theme as Customer: sidebar on `md+`, compact in-page nav on smaller screens, 2-column cards from `sm`. Dashboard shows inbox/proposal/deal/execution slices from live GETs plus profile field presence (not invented KPIs). Deal groups appear only for statuses present on `GET /api/provider/deals` (domain create currently sets `Active`). Execution list groups Pending / InProgress (shown as Started) / Completed / Cancelled. Detail, execution writes, and catalog editing are not implemented (Task 11.4 not started). Business remains a placeholder shell.

## Auth / workspaces

- Public: `/`, `/login`
- Session: `RequireAuth` on `/customer/*`, `/provider/*`, `/business/*`
- Workspace: `RequireWorkspace` using JWT **role names**, not permission claims:
  - `USER` → Customer
  - `USER` + `PROVIDER` → Customer + Provider
  - `USER` + `BUSINESS_OWNER` → Customer + Business
  - `PROVIDER` alone → Provider (Customer is not implied)
  - `ADMIN` → Customer + Provider + Business (no Admin UI)
- Roles are read from the access token (`ClaimTypes.Role` URI and/or JWT `role` / `roles`) in `decodeAccessToken`. `PROVIDER_*` permission claims must **not** unlock the Provider shell. After refresh, the session is rebuilt from the persisted JWT only.

OTP: `POST /api/auth/send-otp`, `POST /api/auth/verify-otp`. `refreshToken` is ignored. 401 on authenticated calls clears the session and redirects to `/login`.

## Design system

Brand colors live in `app/designTokens.ts` (`matchiColors.primary` `#2563EB`, `secondary` `#14B8A6`, plus background/surface/text/mutedText/border/success/warning/error). `app/theme.ts` maps them to MUI `palette`. Components must use `primary.main`, `secondary.main`, `background.default`, `background.paper`, `text.primary`, `text.secondary`, `divider` — not raw hex. Changing primary/secondary in `designTokens.ts` retints the whole app.

Reuse `shared/ui`: `AppCard`, `PageHeader`, `PageContainer`, `FormSplitLayout`, `MarketplaceStepper`, `JourneyTimeline`, `PriceSummary`, `EmptyState`, `LoadingState`, `ErrorAlert`, `StatusChip`, `SectionHeader`. Homepage-only pieces live under `features/shell/home/`. Create-request and create-proposal use a two-column form + summary on `md+`. Steps on create-request are frontend presentation only; submit is still a single `POST /api/requests`. Customer proposal/deal screens use `JourneyTimeline` for Request → Matching → Proposal → Deal → Execution → Review.

Typography lives on the MUI theme (`app/theme.ts`): Vazirmatn (loaded in `index.html`), RTL `fa-IR`. Roles: `h1` display, `h2` section, `h3` subsection, `h4` page title (`PageHeader`), `body1`/`body2` body/secondary, `caption`, `button`.

Layouts: `AppShellLayout` (Customer/Provider/Business) uses a permanent drawer from `md` up and a temporary drawer below; main content is capped (`lg` 1120px / `xl` 1280px). `PublicLayout` has sticky `PublicHeader` + `PublicFooter`. Login stays `sm`; public home is full-width sections with `lg` containers.

## Public homepage

Route `/` (`PublicHomePage`). Sections: header, hero + search, categories, how it works, featured professionals (mock), why Matchi, CTA, footer.

Search / Find a Service goes to `/customer/requests/create?q=` when the session has Customer (`USER`/`ADMIN`), otherwise `/login?next=…` (`next` must be an internal path). Catalog suggestions use `GET /api/services` via MUI Autocomplete (loading/empty/error helpers). Categories prefer `GET /api/services/categories`; empty or failed responses fall back to `shared/mocks/homeMocks.ts`. Featured professionals are mock only (`GET /api/providers` is empty without `serviceId`). Sample cards use **Start a request** (existing find-service path) — there is no public provider profile route.

Create request (`/customer/requests/create`) is a six-step presentation wizard (need → type → describe → location copy → details → review) posting once to `POST /api/requests`. Desktop keeps form + sticky summary; mobile uses a compact progress bar and hides the side summary. After success, the page stays on create with view-request / view-matches / create-another actions instead of navigating away immediately.

My Requests (`/customer/requests`) lists owner requests from `GET /api/requests/me`. Cards show title, kind, status (`Open` / `Cancelled`), id, description snippet, location summary when present, created date, and line counts. Detail (`GET /api/requests/{id}`) shows the same DTO plus schedule, line attributes, and a primary View matches action for Open requests (`/customer/requests/:id/matches`). No extra request fields are invented.

Matches (`/customer/requests/:id/matches`) is read-only. Context comes from `GET /api/requests/{id}`. Results come from `GET /api/requests/{id}/matches` (`candidateType`, `candidateId`, `displayName`, `score`, `rank`). Count is the array length. Cancelled requests do not call matches.

Provider marketplace (`/provider/requests`, alias `/provider/marketplace`) lists `GET /api/provider/requests` (`ProviderRequestInboxItemDto`). Detail (`/provider/requests/:requestId`) reads that list from the query cache; there is no provider `GET /api/requests/{id}`. Open requests can open Create Proposal (`POST /api/requests/{id}/proposals` as `proposerType: "Provider"`). Cards show only inbox fields. Customer identity and street address are not on the contract.

Every data-driven block uses `LoadingState` / `EmptyState` / `ErrorAlert` (API category errors are hidden when mock fallback is used).

## Related docs

- [Folder conventions](./folder-conventions.md)
- [Routing conventions](./routing.md)
- [i18n rules](./i18n.md)
- [API usage rules](./api-usage.md)
- [Provider workspace API audit](./provider-workspace-api-audit.md)
