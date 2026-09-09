# MATCHI_PROJECT_CONTEXT v1.4

**Updated:** 2026-09-09  
**Scope:** Architectural baseline and implementation log for the Matchi .NET 8 marketplace.

This file is the documented Matchi baseline and decision log. It was **not present in the repository at the start of Task 03**. Task 03 therefore treated the Task 02 Request implementation, existing Domain/EF baseline, `docs/api-endpoints-mvp.md`, and the Task 03 specification as the source of truth, then created this file as the required living context.

---

## Marketplace model (authoritative)

```text
Customer
   ↓
Request
   ↓
Matching
   ↓
Provider / Business
   ↓
Proposal
   ↓
Deal
   ↓
ServiceExecution → ExecutionAssignment → Start → Complete
   ↓
Review
```

- There is **no Seller** entity.
- A **Provider** may operate independently, with no Business.
- A **Provider** may be a member of **one or more Businesses**.
- A **Business** may have multiple Providers.
- `BusinessProvider` is membership only. It does not convert a Provider into a Business and does not transfer ownership of Provider services/products.
- The Request is **customer-owned**. Provider/Business targeting happens later in Matching/Proposal.
- Proposal is implemented (Task 05). Deal foundation is implemented (Task 06). Execution and Review are implemented (Task 07). ProductDelivery, Payment, Commission, Chat, Complaint, Verification, and TrustScore wait for later tasks.

---

## Implementation status

| Task | Status |
|---|---|
| Task 01 — Legacy cleanup | COMPLETED (assumed; Loan/Question/Option/RequestAnswer Application APIs not restored) |
| Task 02 — Request Foundation | COMPLETED |
| Task 03 — Provider & Business | COMPLETED |
| Task 04 — Matching | COMPLETED |
| Task 05 — Proposal | COMPLETED |
| Task 06 — Deal | COMPLETED |
| Task 07 — Execution & Review | COMPLETE |
| Task 08 — Migration & Final Hardening | NOT STARTED |

---

## EF / MatchiDb baseline (authoritative)

**Live `MatchiDb` is the source of truth.** The EF model and migration chain must represent the existing database. The database must not be rewritten to match old migrations.

### Decisions

- The live schema is an externally created marketplace schema. It is **not** the result of applying `InitialCreate` + `MatchiBaselineAlignment`.
- Those historical migrations (plus `Task07ExecutionReviewIndexes`) are **incompatible** with live `MatchiDb` and must **not** be replayed. They are archived at `Matchi.Infrastructure/Persistence/Migrations/ArchivedIncompatible/` and excluded from compilation.
- Active compiled migration assembly contains:
  - `20260909065436_MatchiDbExistingBaseline` (stamped; `Up()` never executed)
  - `20260909070502_Task07IndexDelta` (applied to live `MatchiDb`)
- That baseline is a **greenfield** `Up()` (creates the existing marketplace schema). It **must not** be executed against live `MatchiDb`.
- `MatchiDbContextModelSnapshot` represents the post–Task 7 index model.

### Stamp (2026-09-09)

- Baseline migration: `20260909065436_MatchiDbExistingBaseline`
- Stamped into `dbo.__EFMigrationsHistory` with `ProductVersion` `8.0.11`
- Baseline `Up()` was **not** executed (`dotnet ef database update` was **not** run)
- Existing database **schema was not changed**
- Existing database **data was not changed**
- The only database write was the single `__EFMigrationsHistory` insert
- After stamp: `__EFMigrationsHistory` has **exactly one row** (`20260909065436_MatchiDbExistingBaseline`)
- User table count remained **54**
- Full index list before vs after stamp: **identical**

### Task 7 delta (applied 2026-09-09)

Migration: `20260909070502_Task07IndexDelta`

Applied with `dotnet ef database update 20260909070502_Task07IndexDelta`. EF applied **only** this pending migration. Baseline `Up()` was **not** executed.

Indexes removed:

- `IX_ServiceExecutions_DealId`

Indexes added:

- `UX_ServiceExecutions_DealId` — unique on `ServiceExecutions.DealId`
- `UX_ExecutionAssignments_Primary` — unique on `ExecutionAssignments.ServiceExecutionId`, filter `([IsPrimary]=(1) AND [Status]=N'Assigned')`
- `UX_Reviews_Deal_Customer_Business` — unique on `(DealId, CustomerId, BusinessId)`, filter `([IsDeleted]=(0) AND [BusinessId] IS NOT NULL)`
- `UX_Reviews_Deal_Customer_Provider` — unique on `(DealId, CustomerId, ProviderId)`, filter `([IsDeleted]=(0) AND [ProviderId] IS NOT NULL)`
- `IX_ExecutionAssignments_ExecutionId_Status` — non-unique on `(ServiceExecutionId, Status)`

Not dropped (does not exist): `IX_ExecutionAssignments_ServiceExecutionId` — still absent after apply.

Archived `20260909060203_Task07ExecutionReviewIndexes` was **not** compiled or applied.

### Final migration history

| MigrationId | ProductVersion |
|---|---|
| `20260909065436_MatchiDbExistingBaseline` | `8.0.11` |
| `20260909070502_Task07IndexDelta` | `8.0.11` |

Exactly two rows. No `InitialCreate` / `MatchiBaselineAlignment` / archived Task07 IDs.

### Schema / data after Task 7 delta

- User tables: **54** (unchanged)
- Index list delta vs pre-apply: dropped `ServiceExecutions|IX_ServiceExecutions_DealId`; added the five Task 7 indexes above. No other index add/drop.
- Row counts unchanged: Businesses 1, Requests 1, Deals 1, ServiceExecutions 1, ExecutionAssignments 1, Reviews 1
- `IX_ServiceExecutions_DealId` = absent
- Task 7 unique/status indexes = present as listed
- `IX_ExecutionAssignments_ProviderId`, `IX_Reviews_DealId`, `IX_Businesses_OwnerUserId` remain present

### Task 8

**NOT STARTED.**

### Build / tests (Task 7 delta session)

- `dotnet build MatchiSolution.sln` — succeeded, 0 warnings, 0 errors
- `dotnet test Matchi.Application.Tests` — 36 passed, 0 failed

---

## Task 02 — Request Foundation (completed)

Customer-owned Request aggregate:

- `Request` → `RequestService` → `RequestServiceAttribute`
- `Request` → `RequestProduct` → `RequestProductAttribute`
- `RequestLocation`, `RequestSchedule`
- Types: `Service` | `Product` | `Hybrid`

APIs (JWT `[Authorize]`, owner via `Customer.UserId`):

- `POST /api/requests`
- `GET /api/requests/me`
- `GET /api/requests/{requestId}`
- `PUT /api/requests/{requestId}`
- `POST /api/requests/{requestId}/cancel` (`Status = Cancelled`)
- `DELETE /api/requests/{requestId}` (soft-delete)

Customer is never accepted from the client. DTOs live in Application (Contracts project is empty).

---

## Task 03 — Provider & Business Foundation (completed)

### Provider profile

Existing `POST /api/providers` is preserved (current user → unique `Providers.UserId`). Added:

| Method | Route | Permission |
|---|---|---|
| POST | `/api/providers` | `PROVIDER_CREATE` |
| GET | `/api/providers/me` | `PROVIDER_VIEW` |
| PUT | `/api/providers/me` | `PROVIDER_EDIT` |
| GET | `/api/providers/{providerId}` | public |
| GET | `/api/providers` | public search (existing, `serviceId` filter) |

Profile fields: name, mobile (from user on create; optional update), description, lat/lng, status, read-only `rating` / `reviewCount` / `completedJobCount`. UserId is never accepted from the client.

### Provider services / products / capabilities / areas / availability

All scoped to the authenticated user's Provider (`404` if none / not owner):

| Resource | Routes |
|---|---|
| Services | `GET/POST /api/providers/me/services`, `PUT/DELETE /api/providers/me/services/{serviceId}` |
| Products | `GET/POST /api/providers/me/products`, `PUT/DELETE /api/providers/me/products/{productId}` |
| Capabilities | `GET/POST /api/providers/me/capabilities`, `PUT/DELETE /api/providers/me/capabilities/{serviceAttributeId}` |
| Areas | `GET/POST /api/providers/me/areas`, `PUT/DELETE /api/providers/me/areas/{areaId}` |
| Availability | `GET/POST /api/providers/me/availabilities`, `PUT/DELETE /api/providers/me/availabilities/{availabilityId}` |
| Memberships | `GET /api/providers/me/businesses` |

Mutations use `PROVIDER_EDIT`. Reads of `/me*` use `PROVIDER_VIEW`.

Validation:

- Service/product must exist and be active.
- No duplicate active `ProviderService` / `ProviderProduct` (`UX_*` filtered unique indexes). Soft-deleted links are reactivated instead of inserting a second row.
- `ProviderCapability` uses `ServiceAttribute` (not Question/Option). Attribute must be active and belong to a service the provider already offers.
- Area types allowed in this task: `City`, `District`, `Province`, `Radius` (schema is a free `nvarchar(30)`; this is an application allow-list). Radius areas require lat, lng, and radius > 0.
- Availability: `DayOfWeek` 0–6, `TimeFrom < TimeTo`. Overlapping **available** slots on the same day are rejected in Application (DB only indexes ProviderId+DayOfWeek, non-unique).

### Business profile

Owner is always `OwnerUserId` from the current user. A user may own **multiple** businesses (index is not unique).

| Method | Route | Permission |
|---|---|---|
| POST | `/api/businesses` | `BUSINESS_CREATE` |
| GET | `/api/businesses` | public list (now persisted, not an empty stub) |
| GET | `/api/businesses/me` | `BUSINESS_VIEW` (array of owned businesses) |
| PUT | `/api/businesses/me` | `BUSINESS_EDIT` |

`PUT /me` and nested `/me/*` resolve the owned business as:

- `businessId` in body or query when provided and owned;
- the single owned business if the user owns exactly one;
- `400` if the user owns multiple and `businessId` is omitted;
- `404` if none / not owned.

Rating/review/completed counts are read-only. `logoMediaId` may be set on update; media upload is not implemented.

### Business services / products / areas / availability / providers

| Resource | Routes |
|---|---|
| Services | `GET/POST /api/businesses/me/services`, `PUT/DELETE .../{serviceId}` |
| Products | `GET/POST /api/businesses/me/products`, `PUT/DELETE .../{productId}` |
| Areas | `GET/POST /api/businesses/me/areas`, `PUT/DELETE .../{areaId}` |
| Availability | `GET/POST /api/businesses/me/availabilities`, `PUT/DELETE .../{availabilityId}` |
| Providers | `GET/POST /api/businesses/me/providers`, `PUT/DELETE .../{providerId}` |

Reads: `BUSINESS_VIEW`. Mutations: `BUSINESS_EDIT`.

Business services also persist `canCustomerChooseProvider`, `minPrice`, `maxPrice` (existing columns).

### BusinessProvider membership

- Only the Business **owner** can add/update/remove membership via `/me/providers`.
- Provider must exist. Duplicate **active** membership is rejected. Soft-deleted membership is reactivated.
- `DELETE` sets `Status = Inactive`, `LeftAt = UtcNow`, and soft-deletes the **membership row only**. The Provider entity and Provider services/products are untouched. The Provider can continue independently and may belong to other businesses (`UX_BusinessProviders_Active` is `(BusinessId, ProviderId)` where `IsDeleted = 0`, not a global unique ProviderId).
- Status values used: `Active`, `Inactive`, `Pending`.
- Legacy `POST /api/businesses/{businessId}/invite-provider` now **persists** membership as `Pending` (owner-checked). `PUT /api/businesses/business-providers/{id}/accept` activates membership if the current user is that Provider (`PROVIDER_EDIT`).

### Authorization

Permission-based `[Authorize(Policy = "...")]` via existing `PermissionPolicyProvider` / JWT `permission` claims. **No ad-hoc `if (role == ...)` checks.**

Seeded permissions:

- Self-service on role `USER` and `ADMIN`: `PROVIDER_VIEW|CREATE|EDIT`, `BUSINESS_VIEW|CREATE|EDIT`
- Catalog admin on `ADMIN` only: `SERVICE_*`, `PRODUCT_*`, plus existing `REQUEST_VIEW`

**Decision:** Managing a Provider’s offered services uses `PROVIDER_EDIT`, not `SERVICE_CREATE` (catalog). Same for products (`PROVIDER_EDIT` / `BUSINESS_EDIT` vs `PRODUCT_*`). `SERVICE_*` / `PRODUCT_*` are seeded for future catalog-admin APIs. Public provider/business search remains unauthenticated.

Handlers still enforce owner/resource identity (`404` for another user’s Provider/Business), so a permission-bearing user cannot edit by guessing IDs.

OTP users receive role `USER` (`EnsureRoleAsync`). **New permission claims appear only after a new token** (re-verify OTP).

---

## Task 04 — Matching (completed)

Read-only candidate search. Does not mutate Request, create Proposal/Deal, or assign Provider/Business.

**Endpoint:** `GET /api/requests/{requestId}/matches`  
Bearer (`[Authorize]` on `RequestsController`). No new Matching permission.

**Ownership:** Request is loaded with `GetOwnedByIdAsync(requestId, currentUserId)`. Missing or non-owned Request → `KeyNotFoundException` → `404`. Cancelled Request (`Status = Cancelled`) → `ValidationException` → `400` (same lifecycle pattern as update/cancel). No candidates → `200` with `[]`.

**Candidates:** `Provider` and `Business`, independently. `BusinessProvider` is not used. Independent Providers match. Member Providers still match as Provider. Businesses with zero members can match from their own catalog.

**Scoring (deterministic constants):**

| Signal | Points |
|---|---|
| Offers at least one requested service | +50 |
| Offers at least one requested product (id or category) | +20 |
| Has at least one requested `ServiceAttribute` capability (Providers only) | +15 |
| Service area city/province/district matches RequestLocation | +10 |
| Availability overlaps RequestSchedule day/time (`DayOfWeek` = .NET Sunday=0) | +5 |

Eligibility: Service/Hybrid requests require a service match. Product-only requests require a product match. Soft-deleted and non-`Active` Providers/Businesses are excluded. Inactive/deleted catalog links are excluded.

**Ranking:** `Score DESC`, then `CandidateType` (Provider before Business), then `CandidateId ASC`. Duplicates collapsed. `Take(50)` is applied to the **merged** list only (at most 50 candidates total).

**Performance:** Two set-based EF queries (EXISTS-style `Any` in SQL), score computed in the query, merge in memory, then global order + `Take(50)`. No per-candidate round-trips. `BusinessProvider` is not queried.

**Tests:** The repository had no test project. Adding `Matchi.Application.Tests` (xUnit) failed: the configured NuGet feed (`repolocal.farafan.ir`) could not restore `Microsoft.NET.Test.Sdk` / xUnit. The test project was not left in the solution so restore/build of MatchiSolution remains clean. Runtime HTTP matching was not executed.

**Task 04 review/fix:** Final result limit is 50 candidates TOTAL (per-side `Take(50)` removed). Cancelled Requests cannot be matched (`400`). No schema/migration changes.

---

## Task 05 — Proposal (completed)

Reuses existing `Proposal` / `ProposalItem` tables. No `RequestProposal`. Request stays customer-owned (no `ProviderId` / `BusinessId` on Request). Matching is **not** authorization to create a Proposal.

**Proposer:** exactly one of `ProviderId` or `BusinessId` (XOR), resolved server-side:

- `ProposerType = Provider` → `Provider.UserId == currentUserId`; `BusinessId = null`
- `ProposerType = Business` → `Business.OwnerUserId == currentUserId`; `ProviderId = null`. Optional `businessId` only disambiguates **owned** businesses (Task 03 `/me` pattern). `BusinessProvider` membership is **not** party authorization.

Client must not send authoritative `ProviderId`, `UserId`, `OwnerUserId`, or `CustomerId`. `RequestId` comes from the route.

**Create validation:** Request exists, not deleted, `Status = Open` (else `400`). Items: Product XOR Service; catalog must exist, be active, and be offered via `ProviderService`/`ProviderProduct` or `BusinessService`/`BusinessProduct`. Prices ≥ 0. Quantity > 0. Initial status `Pending`. `ExpireAt` may be stored; **no** expiration jobs. Duplicate proposals from the same party are allowed (no unique index).

**Accept / Reject:** Request owner only (`Request.Customer.UserId`). `Pending → Accepted` or `Pending → Rejected`. Invalid transitions → `400`. Request must be `Open` to accept (`400` otherwise). Request status is not changed. Sibling Pending proposals are not auto-rejected.

**Accept (Task 06):** one `SaveChanges` sets Proposal `Accepted` and inserts Deal `Active` (`UX_Deals_ProposalId`). Response includes `dealId`. Reject does not create a Deal.

**APIs** (`[Authorize]` + resource identity; no `PROPOSAL_*` permissions):

| Method | Route | Who |
|---|---|---|
| POST | `/api/requests/{requestId}/proposals` | Provider or Business owner of the proposer |
| GET | `/api/requests/{requestId}/proposals` | Request owner |
| GET | `/api/proposals/{proposalId}` | Request owner of the related Request |
| POST | `/api/proposals/{proposalId}/accept` | Request owner |
| POST | `/api/proposals/{proposalId}/reject` | Request owner |

Non-owned resources → `404`. No migration.

---

## Task 06 — Deal (completed)

Reuses existing `Deals` table. No `ProviderId` / `BusinessId` on Deal. Party is `Deal → Proposal → Provider XOR Business`. `BusinessProvider` is not used. No `POST /api/deals`.

**Create:** `POST /api/proposals/{proposalId}/accept` (Request owner). Requires Request `Open` and Proposal `Pending`. Copies `RequestId`, `ProposalId`, `CustomerId` (from `Request.CustomerId`), `TotalPrice` from Proposal. `Status = Active`, `AcceptedAt = UtcNow`. One `SaveChanges` for Proposal + Deal. Duplicate `ProposalId` (`UX_Deals_ProposalId`) → `400`. Soft-deleted Deal still occupies that unique index (Task 08). Multiple Deals per Request remain possible. Request stays `Open`.

**Read** (`[Authorize]`, customer via `Request.Customer.UserId`; no `DEAL_*` permissions):

| Method | Route |
|---|---|
| GET | `/api/deals` |
| GET | `/api/deals/{dealId}` |

Non-owned → `404`. No proposer Deal APIs. Deal cancel / complete and ProductDelivery are not implemented.

---

## Task 07 — Execution & Review (completed)

**Task 7 — COMPLETE**

Reuses existing `ServiceExecutions`, `ExecutionAssignments`, and `Reviews` tables. `BusinessProvider` is membership only (assignment eligibility), not visibility or party authorization. ProductDelivery is **not** implemented.

### ServiceExecution

Create is allowed only for an **Active** Deal whose Request is `Service` or `Hybrid`, by the Proposal Provider **or** Business owner. `BusinessId` is copied from `Proposal.BusinessId` (null for independent Provider). One execution per Deal (`UX_ServiceExecutions_DealId`). Domain owns transitions:

| From | Action | To | Timestamps |
|---|---|---|---|
| (new) | Create | Pending | — |
| Pending | UpdateSchedule | Pending | `ScheduledTimeFrom < ScheduledTimeTo` when both set |
| Pending | Start | InProgress | `StartedAt = UtcNow` |
| InProgress | Complete | Completed | `CompletedAt = UtcNow` |
| Pending / InProgress | Cancel | Cancelled | Assigned assignments cancelled in the same `SaveChanges` |

Schedule / cancel: Proposal party only. Start / complete: Proposal party **or** primary `Assigned` execution Provider.

### ExecutionAssignment

Business-owner only. Execution must belong to a Business Proposal. Assigned Provider must exist and have **active** `BusinessProvider` membership on that Business. First assignment becomes primary if none exists; a second primary is rejected (`UX_ExecutionAssignments_Primary` filtered unique). Remove sets `Status = Cancelled` while execution is Pending. Cancelled rows remain visible on GET.

### Review

Customer who owns `Deal.Request` only. Deal must be `Active`. Service/Hybrid requires a **Completed** ServiceExecution. Target is XOR: Proposal Business, or Proposal Provider / any assigned execution Provider. Rating 1–5 (`CK_Reviews_Rating`). At least one target (`CK_Reviews_Target`). Duplicate non-deleted review per Deal+Customer+target (`UX_Reviews_Deal_Customer_Business` / `UX_Reviews_Deal_Customer_Provider`). GET lists exclude `IsDeleted`.

### Authorization / visibility

Customer (Request owner), Proposal Provider, Business owner, and assigned Providers can **read** executions/assignments. Mutations do not use membership as authorization. JWT `[Authorize]` on Deal/Execution write paths; public GET reviews unchanged.

### APIs

| Method | Route | Who |
|---|---|---|
| GET | `/api/deals/{dealId}/executions` | Visible party |
| POST | `/api/deals/{dealId}/executions` | Proposal Provider or Business owner |
| GET | `/api/executions/{executionId}` | Visible party |
| PUT | `/api/executions/{executionId}` | Proposal party (schedule) |
| POST | `/api/executions/{executionId}/start` | Party or primary assignee |
| POST | `/api/executions/{executionId}/complete` | Party or primary assignee |
| POST | `/api/executions/{executionId}/cancel` | Proposal party |
| GET/POST | `/api/executions/{executionId}/assignments` | Read: visible; write: Business owner |
| DELETE | `/api/executions/{executionId}/assignments/{assignmentId}` | Business owner |
| POST | `/api/deals/{dealId}/reviews` | Deal customer |
| GET | `/api/providers/{providerId}/reviews` | public |
| GET | `/api/businesses/{businessId}/reviews` | public |

### DB constraints (Task 07)

Live `MatchiDb` now has unique Deal execution (`UX_ServiceExecutions_DealId`), filtered unique primary assignment (`UX_ExecutionAssignments_Primary`), filtered unique review targets, and `IX_ExecutionAssignments_ExecutionId_Status`. Applied via `20260909070502_Task07IndexDelta`. Archived `Task07ExecutionReviewIndexes` was not used. Existing `CK_ServiceExecutions_Time`, `CK_Reviews_Rating`, `CK_Reviews_Target` unchanged.

### Tests / verification

`Matchi.Application.Tests` (xUnit): domain transitions, create/schedule/start/complete/cancel, assignment membership/primary/remove, review create/query including deleted exclusion.

---

## Conflicts with existing code (Task 03)

| Conflict | Resolution |
|---|---|
| `MATCHI_PROJECT_CONTEXT_v1.1.md` missing | Created this file from Task 02/03 baseline. |
| Invite/accept were stubs | Invite persists `Pending` membership; accept activates for the Provider user. Canonical APIs are `/api/businesses/me/providers`. |
| `GET /api/businesses` returned an empty list | Now lists non-deleted businesses. Pagination envelope is still not applied. |
| Only `REQUEST_VIEW` was seeded | Seeded Provider/Business/Service/Product permissions and role `USER`. |
| `Provider`/`Business` children had no mutators | Added domain methods; private setters kept. |
| `AuditableEntity` had no restore | Added `Restore()` for reactivation behind unique filtered indexes. |

---

## Known limitations

- Matching is a read-only query. Proposal, Deal, ServiceExecution, ExecutionAssignment, and Review write/read paths are implemented. ProductDelivery, Deal cancel/complete, chat, complaint, payment, commission, verification, TrustScore: **not implemented**.
- `UX_Deals_ProposalId` is not filtered; a soft-deleted Deal still blocks another Deal for that Proposal.
- Sibling Pending proposals are not auto-rejected. Multiple Deals per Request are allowed.
- Historical Task 05 Accepts may have `Accepted` Proposal with no Deal row.
- Proposal expiration processing and uniqueness of (Request, Provider/Business) are deferred.
- Provider/Business `Rating` / `ReviewCount` denormalized counters are not updated when a Review is created.
- Provider public search still ignores radius/sort/true pagination.
- Business public list ignores page/pageSize.
- `AreaType` allow-list is Application-level, not a DB check constraint.
- Overlap rejection for availability is Application-level.
- `ProviderCapability` DB index is non-unique; uniqueness of one active row per `(ProviderId, ServiceAttributeId)` is enforced in Application.
- Logo/media upload and portfolio APIs are not in this task.
- Task 7 unique indexes are **on live `MatchiDb`** via `20260909070502_Task07IndexDelta`. Historical `Task07ExecutionReviewIndexes` remains archived and was not applied. Broader hardening remains Task 08.
- Runtime HTTP/Swagger UI was **not** executed (no running API/database in this session).
- Existing JWTs issued before the new seed will lack Provider/Business permission claims until re-login.

---

## Verification

| Check | Result |
|---|---|
| `dotnet restore` | Succeeded |
| `dotnet build MatchiSolution.sln` | Succeeded, **0 warnings, 0 errors** |
| Swagger | Swashbuckle is referenced by `Matchi.Api`; the API project compiled. Swagger UI was **not** opened at runtime. |
| Runtime/API tests | **Not performed** |
| Database / `dotnet ef database update` | Baseline `Up()` never executed. `20260909070502_Task07IndexDelta` **was applied**. History: baseline stamp + Task 7 delta. |
| Guid/long | Request/Provider/Business IDs remain `long`. |
| Legacy Loan/Question/Option/RequestAnswer | Not reintroduced in Application/API for this task. |
| Independent Provider | Create Provider does not require a Business. Removing membership does not delete Provider data. |
| Multiple Business memberships | Unique index is per `(BusinessId, ProviderId)` not per Provider. |
| Task 04 Matching | Application query + API compiled. Runtime HTTP not executed. |
| Task 05 Proposal | Application + API compiled. No migration. Runtime HTTP not executed. |
| Task 06 Deal | Accept creates Deal; GET list/detail customer-owned. No migration. Runtime HTTP not executed. |
| Task 07 Execution & Review | Application + API compiled. Unique indexes applied via `Task07IndexDelta`. `Matchi.Application.Tests` 36 passed. Runtime HTTP not executed. |

---

## Seed changes (Task 03)

- Role `USER` is created if missing (required for OTP `EnsureRoleAsync`).
- Permissions listed above are created if missing and mapped to `ADMIN`; self-service set also mapped to `USER`.
- Existing plumbing providers + sample Business + two `BusinessProvider` rows are unchanged.
- No loan/question seed restored.
