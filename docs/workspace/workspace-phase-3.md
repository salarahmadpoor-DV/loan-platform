# Matchi Workspace Phase 3

## Overview

Phase 3 adds persisted **preferred** and **last** workspace selection for authenticated users. The backend is the source of truth for which workspace should open after login or session restore. Switching workspaces updates last-used state only. Setting a default updates preferred (and last) explicitly.

## Problem statement

Users may have Customer, Provider, and/or Business Owner access. Login previously inferred a home from JWT roles plus live capability probes, preferring Provider when present. Notification `actionUrl` values can point at another workspace (`/provider/invitations` from a Customer session), so `RequireWorkspace` could block navigation. Phase 3 stores selection hints on the user, resolves a startup workspace on the server, and switches last-workspace before cross-workspace notification navigation.

### Discovered pre-Phase-3 flow

- OTP login always ensures JWT role `USER`. Provider access is a `Providers.UserId` row, not `role == PROVIDER`. Business workspace is owned businesses (`Businesses.OwnerUserId`), matching `/api/businesses/me`. Membership does not grant Business.
- Frontend `useWorkspaceAccess` called `GET` provider profile and `GET /api/businesses/me` to build capabilities, then `resolveWorkspaces` + `defaultWorkspacePath` (Provider-first fallback).
- `RequireWorkspace` allowed a route only when `canAccess(workspace)` was true; otherwise it redirected to `defaultPath`.
- AppShell account menu and drawer switched by `navigate(workspaceHome[ws])` with no persistence.
- `/app` and Login used that client-side `defaultPath`.
- Phase 2 notifications navigated to a safe in-app `actionUrl` without changing workspace.

## Workspace model

Canonical identifiers (lowercase):

```text
customer
provider
business
```

Same tokens are used in the database, APIs, and `AppWorkspace` on the frontend. Mixed-case values are normalized with `ToLowerInvariant`; unknown tokens are ignored.

## Available workspace types

| Id | Access rule |
| --- | --- |
| `customer` | User has role `USER` (assigned at OTP verify). |
| `provider` | Non-deleted `Provider` whose `UserId` is the current user. |
| `business` | At least one non-deleted `Business` with `OwnerUserId` = current user. |

Role `ADMIN` receives all three identifiers so existing admin tooling keeps working. JWT `PROVIDER` / `BUSINESS_OWNER` codes do **not** grant those workspaces.

## PreferredWorkspace vs LastWorkspace

| Field | Meaning |
| --- | --- |
| **PreferredWorkspace** | Explicit default. Does not change when the user only switches workspace. |
| **LastWorkspace** | Most recently used workspace after an explicit switch (or after setting preferred). |

`PreferredWorkspace != LastWorkspace` is valid and expected.

Neither field grants authorization. They are selection hints. Access is always recomputed from roles and records.

## Workspace access resolution

`IWorkspaceAccessService` / `WorkspaceAccessService` lists currently valid workspaces in a **fixed** order (not database order):

```text
customer → provider → business
```

## Resolution precedence

`WorkspaceResolver.Resolve`:

1. Preferred, if it is in the available list
2. Last, if it is in the available list
3. First entry of the available list (deterministic fallback)
4. `null` if the user has no valid workspace (existing “no workspace” behavior: public `/`)

Stale stored values (lost Provider, lost Business, unknown strings) are skipped.

## Backend API

All routes use `ICurrentUserService`. The client cannot target another user.

| Method | Path | Body | Result |
| --- | --- | --- | --- |
| `GET` | `/api/users/me/workspace` | — | `{ availableWorkspaces, preferredWorkspace, lastWorkspace, resolvedWorkspace }` |
| `PUT` | `/api/users/me/workspace-preference` | `{ workspace }` | Same payload; sets preferred **and** last if `workspace` is available |
| `PUT` | `/api/users/me/workspace-last` | `{ workspace }` | Updates last only; preferred unchanged |

Unavailable or unknown `workspace` → FluentValidation (`workspace` is not available). Unauthorized if there is no current user.

Persistence: nullable `Users.PreferredWorkspace` and `Users.LastWorkspace` (`nvarchar(20)`). Migration `20261003113000_AddUserWorkspacePreferences`.

## Frontend behavior

- `GET /api/users/me/workspace` is the workspace query (`queryKeys.workspace`).
- `useWorkspaceAccess` maps `resolvedWorkspace` to `workspaceHome` for login and `/app`.
- `RequireWorkspace` still checks **available** workspaces only (not preferred).
- Switch: `PUT workspace-last` then navigate to that workspace home.
- Set as default: `PUT workspace-preference` then navigate to that home.
- Account menu (desktop) and drawer settings (mobile) show workspaces, a Default caption, and **Set as default** for the current workspace when it is not already preferred.

## Login / session

After login, refresh, or `/app`, the client waits until workspace state is fetched, then opens `resolvedWorkspace`. It does not default to Customer and does not re-implement the resolver in React.

## Workspace switching

Switch verifies access via the available list, persists last, does not change preferred.

## Notification navigation

Safe `actionUrl` handling from Phase 2 is unchanged. Before navigate:

1. Derive required workspace from the path prefix (`/customer`, `/provider`, `/business`).
2. If missing or already current, navigate.
3. If the user lacks that workspace, do not navigate; show an info message.
4. If they have access, persist last, then SPA-navigate.

No new notification columns were added.

## Authorization / security

- Guards remain capability-based.
- Preferred/last never bypass `RequireWorkspace`.
- Preference APIs bind to the authenticated user only.

## Cache / session isolation

`resetWorkspaceCapabilityQueries` removes `queryKeys.workspace.all` (and existing provider/business/notification keys) on logout, 401, and new session.

## Mobile UX

Header is unchanged (no extra chrome). Workspace switch and set-default live in the existing drawer settings group. Touch targets stay 44px list rows.

## Testing

Application tests in `WorkspaceHandlerTests.cs` cover resolver precedence, only-one-workspace cases, inaccessible set-preferred/set-last, switch last vs preferred, stale preferred, authenticated-user scoping, and provider-record vs role.

Frontend: `npm run typecheck` and `npm run build` (no frontend test script).

## Known limitations

- Customer access still follows JWT `USER`, not a Customer row.
- Fallback order is now customer-first; it differs from the old client Provider-first default.
- Notification workspace is inferred from `actionUrl` prefixes, not stored metadata.
- Workspace GET is an extra authenticated request at shell load.

## Future improvements

- Optional `requiredWorkspace` on notifications
- Set default from any listed workspace in one tap (not only current)
- Include workspace state in the auth payload to avoid a round-trip
