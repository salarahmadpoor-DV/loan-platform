# matchi.web

Independent Matchi marketplace frontend (React, TypeScript, Vite, MUI, TanStack Query, Zustand, Axios, React Router).

It is **not** part of `dotnet build`. At runtime it calls the Matchi HTTP API.

## Architecture overview

Feature-based UI under `src/features`. Shared HTTP, auth, i18n, and design-system primitives live under `src/shared`. Layouts are workspace shells (Customer / Provider / Business). There is no Seller shell.

Request kinds are **Service**, **Product**, and **Hybrid**. The UI must not assume Service-only.

Default UI locale is **fa-IR** (Persian, RTL). Copy is keyed in `src/shared/i18n` so another locale can be added later. Language is **not** chosen from IP or the browser in this version.

## Folder structure

```text
src/
  app/                 # providers, router, theme, query client
  features/
    auth/              # OTP login
    customer/
      dashboard/
      requests/        # list, detail, create
      matching/        # request matches (read-only)
      proposals/       # customer proposal list/detail/accept
      deals/           # customer deal list/detail + read-only execution
      reviews/         # deal review section + /customer/reviews hub
    provider/
      dashboard/
      requests/        # matching inbox list
      proposals/       # my proposals list
      deals/           # provider-visible deals list
      executions/      # involved executions list
      profile/         # GET /api/providers/me
    shell/             # public + business placeholders
  shared/
    api/               # axios client, query keys, errors
    auth/              # session, JWT decode, route guards
    i18n/              # message catalogs + t()
    navigation/
    ui/                # MUI design-system wrappers
    types/
  layouts/
```

## Run commands

From `frontend/matchi.web`:

```bash
npm install
npm run dev
npm run typecheck
npm run build
npm run preview
```

## Environment variables

Copy `.env.example`. Vite exposes:

| Variable | Purpose |
|---|---|
| `VITE_API_BASE_URL` | Matchi HTTP API origin (no trailing slash required). Example: `http://localhost:5000` |

The browser must be allowed by API CORS at deployment. CORS is not configured in the API repo by default.

## Documentation

Detailed frontend rules live in [`docs/`](./docs/README.md) (architecture, folders, routing, i18n, API usage, Provider API audit).

Do **not** invent HTTP endpoints. Provider list GETs are `GET /api/provider/*` plus `GET /api/providers/me`. See `docs/provider-workspace-api-audit.md`.

## Localization strategy (future)

- **Now:** `DEFAULT_LOCALE = fa-IR`. `applyDocumentLocale` sets `lang` / `dir`. MUI theme `direction` is `rtl`. All user-facing strings should go through `t(key)` and catalogs in `src/shared/i18n/locales/`.
- **Later:** add locale catalogs (en-US already exists as a parallel catalog), a user/settings switch, and recreate the MUI theme for LTR. Do **not** add IP-based detection unless product asks for it.
- API validation messages from the backend may still be English until the API is localized.
