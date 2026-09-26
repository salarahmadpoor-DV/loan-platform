# Business Management

This document describes the **implemented** Business ownership and team-management behavior in Matchi. It is not a wishlist.

## 1. Authentication model

There is **no** separate Business Owner login and **no** `BUSINESS_OWNER` JWT role used as identity.

Users sign in with the existing OTP + JWT flow (`POST /api/auth/send-otp`, `POST /api/auth/verify-otp`). The JWT carries the authenticated User (`sub` / name identifier). Workspace access is **not** derived from a Business Owner role.

A Business Owner is simply an authenticated User who owns at least one row in `Businesses` where `OwnerUserId` equals that User’s id.

## 2. Business ownership

Ownership is stored on the Business entity:

```text
Businesses.OwnerUserId = authenticated User.Id
```

`GET /api/businesses/me` returns businesses for the current user via `GetByOwnerUserIdAsync`. The SPA treats `ownsBusiness` as `owned.length > 0` (see `useWorkspaceAccess`). JWT codes named `BUSINESS_OWNER` are ignored for identity (`workspaces.ts`).

The client **cannot** send `OwnerUserId`. `CreateBusinessCommandHandler` sets it from `ICurrentUserService.UserId`.

## 3. Provider relationship

`BusinessProvider` is the membership link between a Business and a Provider (many-to-many). It is **not** ownership.

- Ownership: `Businesses.OwnerUserId`
- Membership: `BusinessProviders` (`BusinessId`, `ProviderId`, `Role`, `Status`, `JoinedAt`, `LeftAt`)

Status values used by membership APIs: `Active`, `Inactive`, `Pending`, `Rejected`.

Removing a member (owner) calls `BusinessProvider.Leave()` (status `Inactive`, soft-delete). Provider **reject** sets status `Rejected` without deleting the row, User, Provider, or Business.

`GET /api/providers/me/businesses` lists memberships for the current Provider. That list does **not** open Business management.

## 4. User combinations

The same User account can be:

| Combination | How it is detected |
|---|---|
| Customer | JWT role `USER` |
| Provider | `Providers.UserId` for this user (`GET /api/providers/me`) |
| Business Owner | at least one `Businesses.OwnerUserId` (`GET /api/businesses/me`) |
| Provider + Business Owner | both of the above |

Customer-only users keep `/customer/*`. Provider-only users keep `/provider/*`. Owning a Business does not remove Provider access (`/provider/requests` still works).

A user may own a Business without a Provider profile. That still enables the existing `/business` shell (`RequireWorkspace workspace="business"`). Primary creation UX in this work is inside the **Provider** workspace.

## 5. Business creation flow

```text
OTP login (existing JWT)
  → Provider workspace (Providers.UserId exists)
  → «کسب‌وکار من» (/provider/business)
  → empty state if GET /api/businesses/me is []
  → «ایجاد کسب‌وکار» (/provider/business/create)
  → POST /api/businesses
  → OwnerUserId = CurrentUserId
  → /provider/business (management)
```

Multiple businesses per owner are allowed (index on `OwnerUserId` is not unique). The UI can select among owned businesses and can create another from the dashboard.

Required create field: **Name**. Optional: description, mobile, address, province, city, district (same as `CreateBusinessCommand`). If mobile is omitted, the handler copies the User’s mobile.

## 6. API endpoints (actual)

Authorization names are permission policies unless noted. Seeded `USER` includes `BUSINESS_VIEW`, `BUSINESS_CREATE`, and `BUSINESS_EDIT`. Ownership is still checked in handlers via `BusinessAccess.RequireOwned`.

| Method | Route | Purpose | Auth | Notes |
|---|---|---|---|---|
| POST | `/api/businesses` | Create business | `BUSINESS_CREATE` | Body: name, description, mobile, address, province, city, district, lat, lng. Response `201` `{ businessId }`. Owner from JWT. |
| GET | `/api/businesses` | Public/list catalog | none on action | Paged marketplace list, **not** “my businesses”. |
| GET | `/api/businesses/me` | Owned businesses | `BUSINESS_VIEW` | `BusinessProfileDto[]` for `OwnerUserId = current user`. Empty array if none. |
| PUT | `/api/businesses/me` | Update owned business | `BUSINESS_EDIT` | Optional `businessId` if the user owns more than one. |
| GET | `/api/businesses/me/providers` | List `BusinessProvider` members | `BUSINESS_VIEW` | Query `businessId` when multiple owned. DTO: providerId, providerName, role, status, joinedAt, leftAt. |
| POST | `/api/businesses/me/providers` | Add membership | `BUSINESS_EDIT` | Body: `providerId`, optional role/status. Default status `Active` if omitted. |
| PUT | `/api/businesses/me/providers/{providerId}` | Update membership role/status | `BUSINESS_EDIT` | |
| DELETE | `/api/businesses/me/providers/{providerId}` | Remove membership | `BUSINESS_EDIT` | Soft-leave only. Query `businessId` when needed. |
| POST | `/api/businesses/{businessId}/invite-provider` | Invite existing Provider | `BUSINESS_EDIT` | Body: `providerId` and/or `mobile`, optional `role`. Creates membership `Pending`. `202` `{ inviteId, status }`. Does not create User/Provider. |
| PUT | `/api/businesses/business-providers/{businessProviderId}/accept` | Accept invite | `PROVIDER_EDIT` | Only the invited Provider (`Provider.UserId`). Pending → Active. `404` if not found / not owner of the invite. |
| PUT | `/api/businesses/business-providers/{businessProviderId}/reject` | Reject invite | `PROVIDER_EDIT` | Same actor check. Pending → Rejected. Does not delete rows. |
| GET | `/api/providers/me/invitations` | Pending invites for current Provider | `ProviderWorkspace` | `{ id, businessName, status, createdAt }[]`. |
| GET | `/api/providers/me/businesses` | Memberships for current Provider | `PROVIDER_VIEW` | Not ownership. |

Also implemented for owned catalog (not the current SPA Business screens): `/api/businesses/me/services|products|areas|availabilities` (GET/POST/PUT/DELETE) with `BUSINESS_VIEW` / `BUSINESS_EDIT` and `RequireOwned`.

## 7. Frontend routes

Provider shell (`RequireAuth` + `RequireWorkspace("provider")`):

| Path | Page |
|---|---|
| `/provider/invitations` | Provider inbox: pending invites, accept/reject |
| `/provider/businesses` | Active `BusinessProvider` memberships (`GET /api/providers/me/businesses`) — not ownership |
| `/provider/business` | Overview of selected owned business, or empty state + create CTA |
| `/provider/business/create` | Create form → `POST /api/businesses` |
| `/provider/business/info` | Edit fields via `PUT /api/businesses/me` |
| `/provider/business/providers` | Team list + invite dialog + remove |
| `/provider/business/invitations` | Members with status `Pending` |

Existing `/business/*` shell remains for users who `ownsBusiness` (same pages for dashboard/info/members/invitations). Catalog and executions under `/business` are still placeholders.

Sidebar: every Provider sees **دعوت‌ها**, **کسب‌وکارها** (memberships), and **کسب‌وکار من** (ownership empty state or management). Owner-only items (info / team / outgoing invites) appear after `ownsBusiness` is true.

## 8. Authorization

- Permission policies gate the HTTP actions (`BUSINESS_*`).
- `BusinessAccess.RequireOwned` loads businesses by `OwnerUserId == currentUserId`. Another user’s `businessId` yields not found.
- Invite/add/remove membership run only after that ownership check.
- Frontend hiding of nav is **not** the security boundary.

## 9. Provider management (`BusinessProvider`)

- List (provider, memberships): `GET /api/providers/me/businesses` — UI at `/provider/businesses` shows **Active** only
- List (owner): `GET /api/businesses/me/providers`
- Invite existing Provider by mobile or `providerId`: `POST /api/businesses/{id}/invite-provider` → `Pending`
- Provider inbox: `GET /api/providers/me/invitations`
- Accept: `PUT /api/businesses/business-providers/{id}/accept` → `Active`
- Reject: `PUT /api/businesses/business-providers/{id}/reject` → `Rejected` (row kept)
- Owner remove: `DELETE /api/businesses/me/providers/{providerId}` → leave/soft-delete membership only

## Provider Invitation Flow

```text
Business owner
  → POST /api/businesses/{businessId}/invite-provider  (BUSINESS_EDIT + RequireOwned)
  → BusinessProvider.Status = Pending
Provider
  → GET /api/providers/me/invitations  (ProviderWorkspace)
  → Accept: PUT .../business-providers/{id}/accept  (PROVIDER_EDIT, Provider.UserId must match)
      → Status = Active
      → Provider stays an independent Provider; membership is additive
  → Reject: PUT .../business-providers/{id}/reject
      → Status = Rejected
      → User, Provider, and Business rows are not deleted
```

Authorization: only the invited Provider can accept/reject (membership `Provider.UserId` vs JWT user). Owners cannot accept on the Provider’s behalf through these endpoints. Re-invite after `Rejected` is allowed; `Active`/`Pending` cannot be duplicated.

## 10. Known limitations

- Invite does not register a new User or Provider if the mobile has no Provider profile.
- Accept and reject are Provider-only APIs (`PROVIDER_EDIT` + `Provider.UserId` match).
- `/business/catalog` and `/business/executions` are placeholders.
- Marketplace `GET /api/businesses` is public listing, unrelated to owner management.
- `useWorkspaceAccess` still exposes a Business **workspace switcher** when the user owns a business; management itself lives under `/provider/business`. That switcher was not rewritten.
