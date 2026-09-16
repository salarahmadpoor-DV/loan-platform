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

## Change log (2026-09-16)

Homepage marketplace IA + centralized blue/teal tokens. Details in [architecture.md](./architecture.md). Mock strategy: `src/shared/mocks/homeMocks.ts`. Remaining TODOs: public featured providers (API needs `serviceId`), public provider profile route, location selector (no public geo search on home).
