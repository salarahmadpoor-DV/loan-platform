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

Provider routes include inbox lists (Task 11.1–11.2) and Provider create-proposal (Task 11.3). Detail, execution writes, and catalog editing are not implemented yet. Business remains a placeholder shell.

## Auth / workspaces

- Public: `/`, `/login`
- Session: `RequireAuth` on `/customer/*`, `/provider/*`, `/business/*`
- Workspace: `RequireWorkspace` using JWT **role names**, not permission claims:
  - `USER` → Customer
  - `PROVIDER` → Customer + Provider
  - `BUSINESS_OWNER` → Customer + Business
  - `ADMIN` → all three (no Admin UI)
- Live tokens often have `USER` (and sometimes `ADMIN`) without `PROVIDER` / `BUSINESS_OWNER`. `PROVIDER_*` permission claims must **not** unlock the Provider shell.

OTP: `POST /api/auth/send-otp`, `POST /api/auth/verify-otp`. `refreshToken` is ignored. 401 on authenticated calls clears the session and redirects to `/login`.

## Design system

Reuse `shared/ui`: `AppCard`, `PageHeader`, `EmptyState`, `LoadingState`, `ErrorAlert`, `StatusChip`. Do not invent parallel primitives.

Every list/detail must handle loading, empty, API error + retry, and 401 (interceptor).

## Related docs

- [Folder conventions](./folder-conventions.md)
- [Routing conventions](./routing.md)
- [i18n rules](./i18n.md)
- [API usage rules](./api-usage.md)
- [Provider workspace API audit](./provider-workspace-api-audit.md)
