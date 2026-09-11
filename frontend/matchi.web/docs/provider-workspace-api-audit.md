# Provider workspace — live API audit

Updated for **Task 10** (backend GETs) and **Task 11.1** (frontend foundation). JWT must include role `PROVIDER` (or `ADMIN`). Permission claims `PROVIDER_VIEW` / `PROVIDER_EDIT` are **not** used by the frontend shell.

Independent Provider vs Business: `BusinessProvider` is membership only. Create-proposal `proposerType: Provider` uses `Provider.UserId`. `proposerType: Business` uses **owned** businesses only (`businessId`), not membership.

Do **not** reuse customer hooks (`useMyDeals`, `useRequest`, `useRequestProposals`) in this workspace.

---

## Foundation screens (Task 11.1)

| Screen | Route | Live API |
|---|---|---|
| Dashboard | `/provider/dashboard` | counts from the four list GETs + optional `GET /api/providers/me` |
| Inbox | `/provider/requests` | `GET /api/provider/requests` |
| My proposals | `/provider/proposals` | `GET /api/provider/proposals` |
| Deals | `/provider/deals` | `GET /api/provider/deals` |
| Executions | `/provider/executions` | `GET /api/provider/executions` |
| Profile | `/provider/profile` | `GET /api/providers/me` |

Empty lists are `200 []`. Missing Provider profile on marketplace lists → **404**. `USER` without role `PROVIDER` is blocked by `RequireWorkspace` and by API **403**.

Inbox DTO (`ProviderRequestInboxItemDto`) fields: `requestId`, `requestType`, `serviceSummary`, `categorySummary`, `location` (`province`/`city`/`district`), `createdDate`, `status`. **No title, no match score, no product-line array.** Task 11.2 shows those fields on `/provider/requests`. `ارسال پیشنهاد` goes to `/provider/requests/:requestId/proposal`. Task 11.3 posts `POST /api/requests/{requestId}/proposals` with `proposerType: "Provider"` (no `businessId` / `providerId`). Catalog pickers use `GET /api/providers/me/services` and `GET /api/providers/me/products`.

---

## Still not in the UI (later tasks)

| Need | Live API | Notes |
|---|---|---|
| Request detail as Provider | none for non-owner | Do not call customer `GET /api/requests/{id}` |
| Create proposal | `POST /api/requests/{id}/proposals` | Task 11.3 form; no owner request GET |
| Proposal / deal / execution detail routes | list DTOs only | No Provider detail GETs beyond lists + `GET /api/executions/{id}` if id known |
| Start / complete execution | existing write APIs | Need ids from the executions list; not wired |
| Catalog edit | `GET/PUT /api/providers/me/services|products|…` | Not in Provider nav in 11.1 |
| Counts endpoint | none | Dashboard counts by listing |

Membership `GET /api/providers/me/businesses` exists but does **not** grant deal visibility and is not shown as a workspace.

---

## What not to do

- Do not invent inbox/deal APIs in the frontend.
- Do not reuse `GET /api/deals` in the Provider workspace.
- Do not implement Business assignment UI in the independent Provider shell.
- Do not add ProductDelivery, payment, or chat.
- Do not change backend/database in this frontend task.
