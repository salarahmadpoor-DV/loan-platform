# Folder conventions

Root of the UI app: `frontend/matchi.web/src`.

## Top level

| Folder | Purpose |
|---|---|
| `app/` | `App.tsx`, `router.tsx`, `theme.ts`, `queryClient.ts`, `providers.tsx` |
| `layouts/` | Workspace shells wrapping `AppShellLayout` |
| `features/` | Feature modules (auth, customer marketplace, shell placeholders) |
| `shared/` | Cross-feature code only |

Do not put page-specific Axios calls in `shared/`. Feature APIs live next to the feature.

## Feature module shape

Use this layout when a feature has HTTP + screens:

```text
features/<workspace>/<feature>/
  api/           types + *Api.ts (getJson / postJson only)
  hooks/         TanStack Query hooks
  components/    presentational MUI
  pages/         route targets
  model/         labels, eligibility, parsing (optional)
```

Examples already in the repo:

- `features/customer/requests/`
- `features/customer/proposals/`
- `features/customer/deals/`
- `features/customer/reviews/`
- `features/provider/proposals/` (list + `create/` form)

Create nested folders only when needed (`requests/create/`). Do not add empty `pages/` or `model/` folders.

## Shared

| Path | Use for |
|---|---|
| `shared/api/httpClient.ts` | Axios instance, Bearer, 401, ProblemDetails |
| `shared/api/queryKeys.ts` | Query key factory; extend namespaces, do not invent ad-hoc keys |
| `shared/api/errors.ts` | `ApiError` |
| `shared/auth/` | Zustand store, JWT decode, `RequireAuth`, `RequireWorkspace` |
| `shared/i18n/` | `keys.ts` + `locales/fa-IR.ts` + `locales/en-US.ts` + `t()` |
| `shared/navigation/navModel.ts` | Sidebar items per workspace |
| `shared/ui/` | Design-system wrappers |
| `shared/types/marketplace.ts` | `RequestKind`, `ProposerType` |

## Naming

- API functions: `getMyDeals`, `createDealReview` — match live paths, not invented names.
- Types: mirror live DTO field names (`camelCase` JSON). Do not add fields the DTO does not return.
- Hooks: `useX` wrapping one query or mutation.
- Pages: `*Page.tsx` exported as named function matching the file.

## What not to do

- No `features/seller/`.
- No Provider UI under `features/customer/`.
- No backend or EF code in this folder.
- Do not copy `node_modules` or `dist` into git.
