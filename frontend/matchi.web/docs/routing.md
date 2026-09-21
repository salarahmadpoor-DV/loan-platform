# Routing conventions

All routes are declared in `src/app/router.tsx`. Do not register routes inside feature folders.

## Public

| Path | Guard | Page |
|---|---|---|
| `/` | none | `PublicHomePage` (marketplace landing; hashes `#categories`, `#how-it-works`, `#for-professionals`) |
| `/login` | none | `LoginPage` (OTP). Optional `?next=/internal-path` after verify (same-origin app paths only). |
| `/provider/onboard` | `RequireAuth` (not Provider workspace) | Become a Professional. `POST /api/providers` for the JWT user. Already a Provider → `/provider/dashboard`. |

## Customer (`RequireAuth` + `RequireWorkspace(customer)` + `CustomerLayout`)

Static segments **before** `:id`.

| Path | Page |
|---|---|
| `/customer` | redirect → `dashboard` |
| `/customer/dashboard` | `CustomerDashboardPage` |
| `/customer/requests` | `RequestListPage` (`GET /api/requests/me`). Title **My Requests**. Cards show DTO fields only; Open requests link to matches. |
| `/customer/requests/create` | `CreateRequestPage` (must stay before `:id`). Optional `?q=` prefills title from homepage search. After `POST /api/requests` the page shows an in-place success card (view request, matches, or create another). |
| `/customer/requests/:id` | `RequestDetailPage` (`GET /api/requests/{id}`). Back to list; View matches when status is Open. |
| `/customer/requests/:id/matches` | `RequestMatchesPage` (`GET /api/requests/{id}/matches` + request context from `GET /api/requests/{id}`). Back to Request. |
| `/customer/requests/:requestId/proposals` | review/compare (`GET /api/requests/{id}/proposals` + `GET /api/proposals/{id}`), accept (`POST .../accept`), reject (`POST .../reject`) |
| `/customer/proposals/:id` | detail + accept/reject |
| `/customer/deals` | `DealListPage` |
| `/customer/deals/:id` | `DealDetailPage` (execution + review) |
| `/customer/reviews` | `ReviewListPage` (deal hub) |

Customer sidebar (plus dashboard home): درخواست‌ها / معاملات / بازخوردها.

## Provider (`RequireAuth` + `RequireWorkspace(provider)` + `ProviderLayout`)

JWT `USER` or `ADMIN`, plus a Provider row for this user. `GET /api/providers/me` uses policy `ProviderWorkspace` (Provider profile or ADMIN), not `PROVIDER_VIEW`. `PermissionPolicyProvider` must map that name to `ProviderProfileRequirement` (not JWT permission `ProviderWorkspace`); otherwise login still 403s and the SPA treats the user as customer-only. Marketplace APIs use the same policy. Default post-login path is `/provider/dashboard` when a Provider profile exists. Creating a profile is `/provider/onboard` (`RequireAuth` only, not this workspace guard).

| Path | Page |
|---|---|
| `/provider` | redirect → `dashboard` |
| `/provider/dashboard` | `ProviderDashboardPage` |
| `/provider/marketplace` | redirect → `/provider/requests` |
| `/provider/requests` | Provider marketplace list (`GET /api/provider/requests`). Read-only cards. |
| `/provider/requests/:requestId` | Provider request detail from inbox DTO (no owner `GET /api/requests/{id}`). |
| `/provider/requests/:requestId/proposal` | create proposal (`POST /api/requests/{id}/proposals`) |
| `/provider/proposals` | my proposals (`GET /api/provider/proposals`) |
| `/provider/deals` | my deals (`GET /api/provider/deals`) |
| `/provider/executions` | my executions (`GET /api/provider/executions`) |
| `/provider/profile` | profile (`GET /api/providers/me`), owned businesses (`GET /api/businesses/me`), memberships (`GET /api/providers/me/businesses`) |

Nav: داشبورد / بازار / پیشنهادهای من / معاملات / اجراها / پروفایل.

Marketplace cards open `/provider/requests/:requestId` (inbox DTO only). Open requests can continue to `/provider/requests/:requestId/proposal` (`POST /api/requests/{id}/proposals`). Do not call owner `GET /api/requests/{id}`.

## Business (`RequireWorkspace(business)` + `BusinessLayout`)

Requires JWT `ADMIN` or at least one owned business (`GET /api/businesses/me`). Membership is not enough.

| Path | Status |
|---|---|
| `/business` | placeholder dashboard |
| `/business/catalog` | placeholder |
| `/business/members` | placeholder |
| `/business/executions` | placeholder |

## Other

| Path | Behavior |
|---|---|
| `/app` | redirect to default workspace (Provider if a profile exists, else first allowed) |
| `*` | redirect `/` |

## Rules

1. Workspace URL prefix matches the shell: `/customer`, `/provider`, `/business`.
2. Nested resources stay under the parent (`/customer/requests/:id/matches`).
3. Cross-resource detail may sit at workspace root when the customer already navigates by id (`/customer/proposals/:id`, `/customer/deals/:id`).
4. Do not add a Customer **Execution** sidebar item; execution stays on deal detail.
5. Do not add Seller routes.
6. Placeholders must remain behind the same guards as real pages.
7. Do not reuse customer `GET /api/deals` / `GET /api/requests/{id}` inside Provider pages.
