# Routing conventions

All routes are declared in `src/app/router.tsx`. Do not register routes inside feature folders.

## Public

| Path | Guard | Page |
|---|---|---|
| `/` | none | `PublicHomePage` |
| `/login` | none | `LoginPage` (OTP) |

## Customer (`RequireAuth` + `RequireWorkspace(customer)` + `CustomerLayout`)

Static segments **before** `:id`.

| Path | Page |
|---|---|
| `/customer` | redirect → `dashboard` |
| `/customer/dashboard` | `CustomerDashboardPage` |
| `/customer/requests` | `RequestListPage` |
| `/customer/requests/create` | `CreateRequestPage` (must stay before `:id`) |
| `/customer/requests/:id` | `RequestDetailPage` |
| `/customer/requests/:id/matches` | `RequestMatchesPage` |
| `/customer/requests/:requestId/proposals` | `RequestProposalsPage` |
| `/customer/proposals/:id` | `ProposalDetailPage` |
| `/customer/deals` | `DealListPage` |
| `/customer/deals/:id` | `DealDetailPage` (execution + review) |
| `/customer/reviews` | `ReviewListPage` (deal hub) |

Customer sidebar (plus dashboard home): درخواست‌ها / معاملات / بازخوردها.

## Provider (`RequireAuth` + `RequireWorkspace(provider)` + `ProviderLayout`)

JWT role `PROVIDER` (or `ADMIN`). Permission claims are not used.

| Path | Page |
|---|---|
| `/provider` | redirect → `dashboard` |
| `/provider/dashboard` | `ProviderDashboardPage` |
| `/provider/requests` | inbox list (`GET /api/provider/requests`) |
| `/provider/requests/:requestId/proposal` | create proposal (`POST /api/requests/{id}/proposals`) |
| `/provider/proposals` | my proposals (`GET /api/provider/proposals`) |
| `/provider/deals` | my deals (`GET /api/provider/deals`) |
| `/provider/executions` | my executions (`GET /api/provider/executions`) |
| `/provider/profile` | profile (`GET /api/providers/me`) |

Nav: داشبورد / درخواست‌ها / پیشنهادهای من / معاملات / اجراها / پروفایل.

Inbox cards navigate to `/provider/requests/:requestId/proposal` with optional inbox state. Do not call owner `GET /api/requests/{id}`.

## Business (`RequireWorkspace(business)` + `BusinessLayout`)

| Path | Status |
|---|---|
| `/business` | placeholder dashboard |
| `/business/catalog` | placeholder |
| `/business/members` | placeholder |
| `/business/executions` | placeholder |

## Other

| Path | Behavior |
|---|---|
| `/app` | redirect to first workspace from JWT roles |
| `*` | redirect `/` |

## Rules

1. Workspace URL prefix matches the shell: `/customer`, `/provider`, `/business`.
2. Nested resources stay under the parent (`/customer/requests/:id/matches`).
3. Cross-resource detail may sit at workspace root when the customer already navigates by id (`/customer/proposals/:id`, `/customer/deals/:id`).
4. Do not add a Customer **Execution** sidebar item; execution stays on deal detail.
5. Do not add Seller routes.
6. Placeholders must remain behind the same guards as real pages.
7. Do not reuse customer `GET /api/deals` / `GET /api/requests/{id}` inside Provider pages.
