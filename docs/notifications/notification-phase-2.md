# Matchi Notification Phase 2

Backend-only persistent notifications from Phase 1 are consumed by the authenticated web app. This phase adds the header bell, unread badge, popover, and Notification Center. Real-time delivery (SignalR), push, email, SMS, and notification preferences are not included.

## API contract

The frontend uses the existing authenticated `httpClient` (Bearer token). It never sends `userId`, `workspaceId`, or `role`. The server resolves the current user.

| Method | Path |
| --- | --- |
| `GET` | `/api/notifications?page=&pageSize=` |
| `GET` | `/api/notifications/unread-count` |
| `POST` | `/api/notifications/{notificationId}/read` |
| `POST` | `/api/notifications/read-all` |

List items used on the client: `id`, `type`, `title`, `message`, `entityType`, `entityId`, `actionUrl`, `isRead`, `createdAt`, `readAt`. `UserId` and `ReferenceKey` are not displayed.

## Notification Bell

The bell is rendered in the AppShell header on **desktop** (`md` and up). It uses `NotificationsNoneOutlined`, a 44×44 `IconButton`, `aria-label` from `nav.notifications`, and header height `56px`.

Unread count comes from `GET /api/notifications/unread-count`. The badge is compact, `max={99}`, and hidden when the count is `0`. There is no emoji.

Opening the popover loads the first **8** notifications (`page=1`, `pageSize=8`). Actions:

- **Mark all as read** — primary (`contained`) when unread count &gt; 0
- Click an item — mark as read if unread, then SPA-navigate when `actionUrl` is a safe in-app path
- **View all** — text button to the workspace Notification Center

The popover is width-capped (`360px`, `max-width: calc(100vw - 16px)`, `max-height: min(70vh, 520px)`) with vertical scroll. Anchor origin follows physical `left`/`right` from `theme.direction` so it stays near the header cluster in RTL.

Polling: unread count refetches every **30 seconds** while the bell is mounted (desktop AppShell). TanStack Query does not poll unobserved queries. Logout unmounts the shell and removes notification queries (see Authentication isolation).

One AppShell is mounted per workspace route tree, so there is a single unread query, not one per workspace component.

## Notification Center

Shared page: `NotificationCenterPage`.

| Workspace | Route |
| --- | --- |
| Customer | `/customer/notifications` |
| Provider | `/provider/notifications` |
| Business | `/business/notifications` |

The same UI is used in all three routes. Layout uses `PageHeader`, `AppCard`, `StatusChip`, `LoadingState`, `EmptyState`, and `ErrorAlert`. Primary action is **Mark all as read**. Individual **Mark as read** is a text button. Load more increases `pageSize` from 20 toward a cap of **100** (API maximum).

Timestamps use existing `formatDateTime` (locale calendar, including Persian on `fa-IR`). Jalali DatePicker was not added.

## Mobile

The header bell is **not** shown below the `md` breakpoint so the header stays hamburger + title + account. Notification Center remains reachable from the workspace **drawer** (More → matching group):

- Customer work group → `/customer/notifications`
- Provider account group → `/provider/notifications`
- Business work group → `/business/notifications`

Bottom navigation was not redesigned.

## actionUrl navigation

Navigation uses `react-router` `navigate` (SPA, no full reload).

`resolveInAppNotificationPath` accepts only trimmed paths that:

- start with `/` and do not start with `//`
- contain no backslashes or control characters
- parse with `new URL(path, window.location.origin)` to the **same origin**
- use `http:` or `https:`

Rejected examples: `https://…`, `http://…`, `//host`, `javascript:…`, protocol-relative and origin-switching URLs. The navigated value is `pathname + search + hash` from the parsed URL.

## Mark as read

Individual: `POST /api/notifications/{id}/read`. On success, cached list items and unread counts are **patched in place** (no full invalidate).

Mark all: `POST /api/notifications/read-all`. On success, visible items are marked read and unread count is set to `0`.

Mutation errors use existing `ErrorAlert` / `ApiError` user messages.

## Loading / empty / error

- Loading: `LoadingState`
- Empty: `EmptyState` with Persian/English copy (`notifications.emptyTitle` / `emptyBody`)
- Error: `ErrorAlert`

## RTL

Header chrome keeps the project `flexDirection: "row"` inline pattern (no extra `row-reverse`). Body text uses start-aligned typography. Popover alignment uses `theme.direction`. Drawer notification links use existing `WorkspaceDrawerNav` / `rtlSafeFlexRow`.

## Accessibility

Bell: `aria-label`, `aria-haspopup="dialog"`, `aria-expanded`. Items are keyboard-activable (`Enter` / `Space`). Touch targets stay at least 44px for header controls.

## Authentication isolation

Notifications belong to the authenticated user, not the selected workspace. The same APIs are used in every workspace.

On logout, 401 handling, and new session, `resetWorkspaceCapabilityQueries` **removes** `queryKeys.notifications.all` so a later login cannot show the previous user’s list or badge.

## Testing

`frontend/matchi.web` has `typecheck` and `build` scripts. There is no frontend unit-test or lint script in `package.json`. Phase 2 validation is `npm run typecheck` and `npm run build`.

## Known limitations

- No SignalR; unread count on desktop polls every 30 seconds while the bell is mounted.
- Mobile has no header badge; users open the drawer (or a known URL).
- `actionUrl` values that point at another workspace may still be blocked by `RequireWorkspace` after navigation.
- Load more is capped at `pageSize` 100; older notifications beyond that are not fetched.
- Business operations and notification persistence may still use separate `SaveChanges` on the server (Phase 1).
- New Match, Outbox, push, email, SMS, and preferences are out of scope.
