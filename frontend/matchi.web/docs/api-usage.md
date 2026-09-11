# API usage rules

The frontend may call **existing Matchi HTTP APIs only**. Do not invent paths, query filters, or DTO fields.

Authoritative live contracts are the controllers under `Matchi.Api/Controllers/` plus Application DTOs. `docs/api-endpoints-mvp.md` is useful but can lag (for example execution APIs exist even where an older “target” section says they do not).

## Client

- Use `getJson` / `postJson` / `putJson` / `deleteJson` from `shared/api/httpClient.ts`.
- Base URL: `VITE_API_BASE_URL`.
- Bearer is attached automatically except `send-otp` / `verify-otp`.
- 401: clear session, redirect `/login`.
- 400 / 409: `ErrorAlert` shows ProblemDetails `errors` (first field message) or `detail`.
- IDs are `long` on the API and `number` in TypeScript. Do not switch to Guid.

## TanStack Query

- Keys come from `queryKeys` in `shared/api/queryKeys.ts`.
- Provider lists use `queryKeys.provider.*`. Do not reuse `queryKeys.deals.mine()` (that is customer `GET /api/deals`).
- `enabled` must guard invalid ids.
- Mutations invalidate the namespaces they change.

## Mapping DTOs

- Copy live JSON property names (`requestType`, `proposerType`, `acceptedAt`, …).
- Optional / missing nested data: show empty state; do not fake display names when the DTO only has an id.
- Compose screens from multiple GETs when one DTO is incomplete (deal detail = deal + request + proposal + executions + assignments). That is allowed. Inventing `GET /api/deals/{id}/full` is not.

## Ownership (do not assume)

Many GETs are **customer-owned** even if the URL looks generic:

| Call | Actual visibility |
|---|---|
| `GET /api/requests/me` | current customer |
| `GET /api/requests/{id}` | request owner only |
| `GET /api/requests/{id}/proposals` | request owner only |
| `GET /api/proposals/{id}` | request owner only |
| `GET /api/deals` / `GET /api/deals/{id}` | **customer** (`Request.Customer.UserId`) only |
| `GET /api/provider/requests` | JWT role `PROVIDER` — matching-eligible open requests |
| `GET /api/provider/proposals` | JWT role `PROVIDER` — own proposals |
| `GET /api/provider/deals` | JWT role `PROVIDER` — own proposal or Assigned executor |
| `GET /api/provider/executions` | JWT role `PROVIDER` — party or Assigned |
| `GET /api/providers/me` | `PROVIDER_VIEW` — current Provider profile |
| `GET /api/deals/{id}/executions` | customer, proposal party, or assigned provider |
| `POST /api/requests/{id}/proposals` | Provider or Business **owner** (not membership). Provider UI sends `proposerType: "Provider"` only. |
| `GET /api/providers/me/services` | `PROVIDER_VIEW` — current Provider service links |
| `GET /api/providers/me/products` | `PROVIDER_VIEW` — current Provider product links |
| `POST /api/deals/{id}/reviews` | deal customer |

If a workspace cannot see a resource, **do not** reuse the customer list hook and hope. See [Provider workspace API audit](./provider-workspace-api-audit.md).

## CORS

Browser calls need API CORS at deployment. The API project does not configure CORS by default.

## Forbidden

- Calling endpoints “for the lab” that are not in the controllers.
- POST execution/review/proposal from a workspace that the handler rejects.
- ProductDelivery, Payment, Chat, Commission — not implemented on the API.
