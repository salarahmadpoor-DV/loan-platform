# matchi.web documentation

Living frontend docs for architecture v2. Product/backend decisions remain in the repo root `MATCHI_PROJECT_CONTEXT_v1.1.md`.

| Doc | Contents |
|---|---|
| [architecture.md](./architecture.md) | Stack, layers, design tokens, homepage, customer flow, workspaces |
| [folder-conventions.md](./folder-conventions.md) | Feature folders and shared code |
| [routing.md](./routing.md) | Routes and guards |
| [i18n.md](./i18n.md) | fa-IR RTL and `t()` |
| [api-usage.md](./api-usage.md) | Axios, query keys, ownership |
| [provider-workspace-api-audit.md](./provider-workspace-api-audit.md) | Provider live APIs vs remaining UI gaps |

Repo-root `docs/provider-service-area.md` describes map-based Provider service area (Leaflet over existing `lat`/`lng`/`Radius` APIs).

Repo-root `docs/catalog.md` describes Service vs Product catalogs. `docs/provider-catalog.md` and `docs/business-catalog.md` describe offering join tables and `/provider/offerings` / `/business/catalog`.

Repo-root `docs/matching.md` describes `GET /api/requests/{id}/matches` scores, city fallback, and Haversine ranking (`distanceKm`).

## Change log (2026-09-16)

Homepage marketplace IA + centralized blue/teal tokens. Details in [architecture.md](./architecture.md). Mock strategy: `src/shared/mocks/homeMocks.ts`. Remaining TODOs: public featured providers (API needs `serviceId`), public provider profile route, location selector (no public geo search on home).

**Homepage UX polish (later same day):** Autocomplete search with loading/empty/error helpers; header hierarchy (Find Services / How it works / Login / Get Started); stacked category grid (no horizontal overflow); sample professional CTA is Start a request, not a fake profile.
