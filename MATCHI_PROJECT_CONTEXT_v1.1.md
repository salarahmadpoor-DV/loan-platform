# MATCHI_PROJECT_CONTEXT v2.0

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
| Task 08 — Migration & Final Hardening | COMPLETE (Phases 1–5, 8.6, 8.7A, 8.7B; Review XOR on live DB) |

---

## EF / MatchiDb baseline (authoritative)

**Live `MatchiDb` is the source of truth.** The EF model and migration chain must represent the existing database. The database must not be rewritten to match old migrations.

### Decisions

- The live schema is an externally created marketplace schema. It is **not** the result of applying `InitialCreate` + `MatchiBaselineAlignment`.
- Those historical migrations (plus `Task07ExecutionReviewIndexes`) are **incompatible** with live `MatchiDb` and must **not** be replayed. They are archived at `Matchi.Infrastructure/Persistence/Migrations/ArchivedIncompatible/` and excluded from compilation.
- Active compiled migration assembly contains:
  - `20260909065436_MatchiDbExistingBaseline` (stamped; `Up()` never executed)
  - `20260909070502_Task07IndexDelta` (applied to live `MatchiDb`)
  - `20260909083411_Task08IntegrityConcurrency` (**applied** 2026-09-09)
  - `20260909092717_Task08ReviewTargetXor` (**applied** 2026-09-09)
- That baseline is a **greenfield** `Up()` (creates the existing marketplace schema). It **must not** be executed against live `MatchiDb`.
- `MatchiDbContextModelSnapshot` represents the post–Task 8.7B model (integrity indexes + XOR `CK_Reviews_Target`). Live `MatchiDb` includes the Task 8.2 index delta and XOR Review check.

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

### Final migration history (live `MatchiDb`)

| MigrationId | ProductVersion |
|---|---|
| `20260909065436_MatchiDbExistingBaseline` | `8.0.11` |
| `20260909070502_Task07IndexDelta` | `8.0.11` |
| `20260909083411_Task08IntegrityConcurrency` | `8.0.11` |
| `20260909092717_Task08ReviewTargetXor` | `8.0.11` |

Exactly four applied rows. No `InitialCreate` / `MatchiBaselineAlignment` / archived Task07 IDs.

### Schema / data after Task 7 delta

- User tables: **54** (unchanged)
- Index list delta vs pre-apply: dropped `ServiceExecutions|IX_ServiceExecutions_DealId`; added the five Task 7 indexes above. No other index add/drop.
- Row counts unchanged: Businesses 1, Requests 1, Deals 1, ServiceExecutions 1, ExecutionAssignments 1, Reviews 1
- `IX_ServiceExecutions_DealId` = absent
- Task 7 unique/status indexes = present as listed
- `IX_ExecutionAssignments_ProviderId`, `IX_Reviews_DealId`, `IX_Businesses_OwnerUserId` remain present

### Task 8

**COMPLETE** for the Task 8 hardening scope (Phases 1–5, 8.6, 8.7A, 8.7B). Product items listed under Known limitations remain deferred and are not Task 8 blockers.

### Task 8 Phase 2 — Security & runtime hardening (2026-09-09)

Implemented in application/configuration only. **No EF migrations. No database schema or data changes. Baseline `Up()` was not executed.**

- Committed `appsettings.json` / `appsettings.Development.json` no longer contain SQL credentials or JWT signing keys. Keys remain `ConnectionStrings:DefaultConnection` and `Jwt:Key` (plus Issuer/Audience). Local Development: .NET User Secrets (`UserSecretsId` on `Matchi.Api`). Deployment: environment variables (`ConnectionStrings__DefaultConnection`, `Jwt__Key`). Credentials that were previously committed **must be rotated outside this repo**.
- OTP: cryptographically random 6-digit codes, TTL (default 5 minutes), max failed attempts (default 5), in-memory store. HTTP contract unchanged (`requestId` only). Static well-known codes are not a bypass.
- Production unexpected exceptions return a generic 500 plus `traceId`. Details are logged server-side. Development may still expose exception type/message. `400`/`401`/`404` mappings preserved.
- `DatabaseSeeder` runs only when `Seed:Enabled=true` **and** the environment is Development. Production never auto-seeds. Default in committed `appsettings.json` is `Seed:Enabled=false`.
- Removed debug route `GET /api/users/request-view-test`.

Developer seeding: Development + `"Seed": { "Enabled": true }` in `appsettings.Development.json` (already set). To disable local seed, set `Seed:Enabled` to `false`.

JWT validation (issuer, audience, lifetime, signing key) is unchanged.

### Build / tests (Task 8 Phase 2)

- `dotnet build MatchiSolution.sln --no-restore` — succeeded, 0 errors (NU1900 nuget feed warnings only)
- `dotnet test Matchi.Application.Tests --no-restore` — 45 passed, 0 failed

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

Customer who owns `Deal.Request` only. Deal must be `Active`. Service/Hybrid requires a **Completed** ServiceExecution. Target is XOR: Proposal Business, or Proposal Provider / any assigned execution Provider. Rating 1–5 (`CK_Reviews_Rating`). Exactly one of `BusinessId` / `ProviderId` (`CK_Reviews_Target` XOR). Duplicate non-deleted review per Deal+Customer+target (`UX_Reviews_Deal_Customer_Business` / `UX_Reviews_Deal_Customer_Provider`). GET lists exclude `IsDeleted`.

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

Live `MatchiDb` now has unique Deal execution (`UX_ServiceExecutions_DealId`), filtered unique primary assignment (`UX_ExecutionAssignments_Primary`), filtered unique review targets, and `IX_ExecutionAssignments_ExecutionId_Status`. Applied via `20260909070502_Task07IndexDelta`. Archived `Task07ExecutionReviewIndexes` was not used. `CK_Reviews_Target` is XOR as of `20260909092717_Task08ReviewTargetXor`. `CK_ServiceExecutions_Time` and `CK_Reviews_Rating` unchanged.

### Tests / verification

`Matchi.Application.Tests` (xUnit): domain transitions, create/schedule/start/complete/cancel, assignment membership/primary/remove, review create/query including deleted exclusion.

---

## Task 8 Phase 3 / 8.2 — Data integrity & concurrency (2026-09-09)

**Status:** COMPLETE for this phase. Task 8 remains IN PROGRESS. Phase 2 (security/runtime) remains COMPLETE and uncommitted with this work.

### Preflight (read-only, live `MatchiDb`)

- `__EFMigrationsHistory`: `20260909065436_MatchiDbExistingBaseline`, `20260909070502_Task07IndexDelta` (unchanged after this phase)
- Review XOR violations `(both null OR both set)`: **1 row** — `Reviews.Id = 1`, `DealId = 1`, `CustomerId = 2`, `BusinessId = 2`, `ProviderId = 3`, `IsDeleted = 0`
- Duplicate Assigned `(ServiceExecutionId, ProviderId)`: **0**
- Duplicate non-deleted Deals per `ProposalId`: **0**
- Live `CK_Reviews_Target`: `[BusinessId] IS NOT NULL OR [ProviderId] IS NOT NULL` (OR)
- Live `UX_Deals_ProposalId`: unique, **unfiltered**
- Executions: 1 Pending. Assignments: 1 Assigned

### Implemented

- Filtered unique `UX_ExecutionAssignments_AssignedProvider` on `(ServiceExecutionId, ProviderId)` where `Status = N'Assigned'`. Cancelled history and later reassignment remain allowed. Application `HasAssignedProviderAsync` + `ConflictException`.
- `UX_Deals_ProposalId` replaced with unique filtered `[IsDeleted] = 0` (one non-deleted Deal per Proposal). Product intent: Deal is `AuditableEntity`; lists already exclude `IsDeleted`.
- `ServiceExecution.Status` is an EF concurrency token (no new column, no `rowversion`). Concurrent Start/Complete/Cancel last-write-wins is replaced by `DbUpdateConcurrencyException` → `ConflictException` → HTTP 409.
- Unique index violations on Deal / execution / assignment / review save map to `ConflictException` (HTTP 409). Validation remains 400. Production 500 sanitization unchanged.

### Deferred in this phase

- **XOR `CK_Reviews_Target` not changed.** Domain still requires exactly one of Business/Provider. Live row 1 has both values; adding XOR would fail on apply. No data rewrite in this task.
- `rowversion` not added (Status token is sufficient for the execution lifecycle race).

### Migration

- Created: `20260909083411_Task08IntegrityConcurrency`
- **Not applied.** `dotnet ef database update` was **not** run. Live schema and data unchanged.

`Up()`:

1. `DropIndex` `UX_Deals_ProposalId` on `Deals`
2. `CreateIndex` unique filtered `UX_ExecutionAssignments_AssignedProvider`
3. `CreateIndex` unique filtered `UX_Deals_ProposalId`

No CreateTable/DropTable/AlterColumn/data operations. Status concurrency is snapshot-only.

---

## Task 8 Phase 4 / 8.3 — Request / Deal lifecycle (2026-09-09)

**Status:** COMPLETE for this phase. Task 8 remains IN PROGRESS. Phases 2 and 3 remain COMPLETE (Phase 3 migration still unapplied).

### Policy implemented

Active Deal = non-deleted Deal with `Status = "Active"` (`Deal.Create` always starts Active; Deal Complete/Cancel is not implemented). Soft-deleted Deals do not block.

- Owner cancel of an Open Request **with** an Active Deal → `ConflictException` / HTTP **409**. Request status stays `Open`. Deal unchanged.
- Owner delete of a Request **with** an Active Deal → **409**. Request not soft-deleted. Deal unchanged.
- Owner cancel **without** an Active Deal → existing `Cancelled` behavior.
- Owner delete **without** an Active Deal → existing soft-delete.
- Non-owner cancel/delete still **404** via `GetOwnedByIdAsync` **before** the Active Deal query (IDOR unchanged).
- Accept Proposal still creates an Active Deal, leaves Request **Open**, does **not** auto-reject siblings, and still allows **multiple** Active Deals per Request.

`IDealRepository.HasActiveDealForRequestAsync` is an existence query (`AnyAsync`). No new index, no schema change.

### Race

Cancel/delete vs concurrent Accept is **not** fully race-free: there is no shared transaction or Request concurrency token. Application guard is required; DB isolation / Request RowVersion is **deferred**.

### BusinessProvider

Owner `POST /api/businesses/me/providers` can still create `Active` membership (optional `status`, default Active). `POST .../invite-provider` still persists `Pending`. Context already documents owner add/update/remove on `/me/providers` and invite as Pending + provider accept. This is **intended membership management**, not an authorization bypass of owner-only APIs. **Left unchanged.** Invite-must-accept as a hard security rule is a product decision and remains deferred.

### Database

No migration created or modified. `20260909083411_Task08IntegrityConcurrency` remains unapplied. Live `MatchiDb` unchanged.

---

## Task 8 Phase 5 / 8.4 — Final application hardening & readiness audit (2026-09-09)

**Status:** COMPLETE for this phase (PASS WITH FINDINGS). Task 8 remains **IN PROGRESS** because the integrity migration is unapplied and Review XOR cannot be enforced in the DB until legacy data is remediated.

### Implemented

- Catalog list paging: `GET /api/services` (`q` + page/pageSize), `GET /api/businesses` (page/pageSize), `GET /api/providers` (page/pageSize when `serviceId` is present). Filter then `OrderBy` then `Skip`/`Take` in SQL. Page size capped at 100.
- Review list mapping skips rows missing the list’s target id (defensive for malformed data). Dual-target legacy rows do not throw.
- `docs/api-endpoints-mvp.md` aligned for paging, 409, review XOR, invite persist, CORS note.

### Audited, not changed

- **CORS:** no frontend origin in repo; not required by current API-only architecture. Deferred to deployment. No `AllowAnyOrigin`.
- **Matching:** read-time only (`GetRequestMatchesQuery` + `IMatchingReadRepository`). No match persistence / write-path. Intentionally deferred.
- **Review XOR DB:** domain + FluentValidation XOR. Live `CK_Reviews_Target` remains OR. `Reviews.Id = 1` still has both targets. No migration, no data change.
- **Authorization:** no new IDOR or missing `[Authorize]` on Task 8 paths. Public catalog/review GETs remain public. Owner 404-before-lifecycle remains.
- **Runtime:** Phase 2 intact (empty committed secrets, JWT required, seed Development-only, sanitized 500 + traceId, Swagger Development-only, random OTP).
- **Dead code:** no RequestViewTest, static OTP, or Console.WriteLine in production projects.
- Provider `lat`/`lng`/`radiusKm`/`sort` still ignored (geo/sort not specified beyond query params).
- Requests/Deals/Reviews/Executions lists were never paginated query contracts.

### Database

No migration created/modified/applied. Live history unchanged.

---

## Task 8 Phase 8.6 — Controlled apply of `Task08IntegrityConcurrency` (2026-09-09)

**Status:** COMPLETE. Task 8 remains IN PROGRESS (Review XOR DB still pending). No application code changes in this phase. No commit. No push.

**Target:** SQL Server `.`, database `MatchiDb`. Command: `dotnet ef database update 20260909083411_Task08IntegrityConcurrency`.

### Preflight (read-only)

- History before apply: baseline + `Task07IndexDelta` only (2 rows). Target ID absent.
- Active Deal duplicates (`IsDeleted = 0`, same `ProposalId`): **0**
- Assigned Provider duplicates (`Status = Assigned`, same `(ServiceExecutionId, ProviderId)`): **0**
- `UX_Deals_ProposalId` existed, unique, **unfiltered**
- `UX_ExecutionAssignments_AssignedProvider` did **not** exist
- `Reviews.Id = 1` still `BusinessId = 2`, `ProviderId = 3` (not modified)

### Applied SQL (EF)

1. `DROP INDEX [UX_Deals_ProposalId] ON [dbo].[Deals]`
2. `CREATE UNIQUE INDEX [UX_ExecutionAssignments_AssignedProvider] ON [dbo].[ExecutionAssignments] ([ServiceExecutionId], [ProviderId]) WHERE [Status] = N'Assigned'`
3. `CREATE UNIQUE INDEX [UX_Deals_ProposalId] ON [dbo].[Deals] ([ProposalId]) WHERE [IsDeleted] = 0`
4. Insert history row `20260909083411_Task08IntegrityConcurrency` / `8.0.11`

Baseline `Up()` was **not** executed. No other pending migrations were applied.

### Post-verify

- History: three rows as listed above
- `UX_Deals_ProposalId`: unique, filter `([IsDeleted]=(0))`
- `UX_ExecutionAssignments_AssignedProvider`: unique, filter `([Status]=N'Assigned')`
- Other Deals/ExecutionAssignments indexes unchanged except the two operations above
- `CK_Reviews_Target` still OR; `Reviews.Id = 1` unchanged; Deal/Assignment/Review row counts still 1
- No tables/columns added or dropped by this apply

### Tests / build

- `dotnet test Matchi.Application.Tests --no-restore`: 67 passed, 0 failed
- `dotnet build MatchiSolution.sln --no-restore`: 0 errors; NU1900 only

---

## Task 8 Phase 8.7A — Review #1 target audit (2026-09-09)

**Status:** COMPLETE (PASS). READ-ONLY. No schema or data change in that phase.

Conclusive target for `Reviews.Id = 1`: **Business** (do not reinterpret).

- Pre-remediation row: `DealId = 1`, `CustomerId = 2`, `BusinessId = 2`, `ProviderId = 3`, `IsDeleted = 0`
- Marketplace party on accepted Proposal 2: `BusinessId = 2`, `ProviderId = NULL`
- Provider 3 is execution-assigned on Deal 1, not the Proposal Provider
- Only dual-target Review: Id 1
- Intended final row: `BusinessId = 2`, `ProviderId = NULL`

---

## Task 8 Phase 8.7B — Review #1 remediation + XOR `CK_Reviews_Target` (2026-09-09)

**Status:** COMPLETE. Task 8 hardening scope COMPLETE. No commit. No push.

**Target:** SQL Server `.`, database `MatchiDb`.

### Data (before constraint)

Preflight: Review 1 was `BusinessId = 2`, `ProviderId = 3`, `IsDeleted = 0`. Dual-target count = **1**.

UPDATE (exactly one row): `ProviderId = NULL`, `UpdateDate = GETDATE()` where `Id = 1 AND BusinessId = 2 AND ProviderId = 3`.

After:

- Review 1: `BusinessId = 2`, `ProviderId = NULL`, `IsDeleted = 0`
- Dual-target: **0**
- Neither-target: **0**
- Business-only: **1**
- Provider-only: **0**

### Schema

EF `ReviewConfiguration` `CK_Reviews_Target` replaced OR with XOR. Domain/FluentValidation already XOR. Provider reviews may still target an execution-assigned Provider (unchanged).

Migration: `20260909092717_Task08ReviewTargetXor`

`Up()` only: drop `CK_Reviews_Target`; add XOR `CK_Reviews_Target`. No table/column/index/FK/data/seed operations.

Applied: `dotnet ef database update 20260909092717_Task08ReviewTargetXor`. Baseline `Up()` was **not** executed.

Live definition: `([BusinessId] IS NOT NULL AND [ProviderId] IS NULL OR [BusinessId] IS NULL AND [ProviderId] IS NOT NULL)` (SQL Server AND-before-OR; equivalent to XOR).

### Tests / build

- `dotnet test Matchi.Application.Tests --no-restore`: 67 passed, 0 failed (no test updates)
- `dotnet build MatchiSolution.sln --no-restore`: 0 errors; NU1900 only

---

## Conflicts with existing code (Task 03)

| Conflict | Resolution |
|---|---|
| `MATCHI_PROJECT_CONTEXT_v1.1.md` missing | Created this file from Task 02/03 baseline. |
| Invite/accept were stubs | Invite persists `Pending` membership; accept activates for the Provider user. Canonical APIs are `/api/businesses/me/providers`. |
| `GET /api/businesses` returned an empty list | Now lists non-deleted businesses. Pagination applied in Phase 8.4. |
| Only `REQUEST_VIEW` was seeded | Seeded Provider/Business/Service/Product permissions and role `USER`. |
| `Provider`/`Business` children had no mutators | Added domain methods; private setters kept. |
| `AuditableEntity` had no restore | Added `Restore()` for reactivation behind unique filtered indexes. |

---

## Known limitations

- Matching is a read-only query. Proposal, Deal, ServiceExecution, ExecutionAssignment, and Review write/read paths are implemented. ProductDelivery, Deal cancel/complete, chat, complaint, payment, commission, verification, TrustScore: **not implemented**.
- Sibling Pending proposals are not auto-rejected. Multiple Deals per Request are allowed.
- Historical Task 05 Accepts may have `Accepted` Proposal with no Deal row.
- Proposal expiration processing and uniqueness of (Request, Provider/Business) are deferred.
- Provider/Business `Rating` / `ReviewCount` denormalized counters are not updated when a Review is created.
- Provider public search still ignores `lat`/`lng`/`radiusKm`/`sort`. Pagination for `serviceId` search is applied.
- CORS is not configured; set allowed origins at deployment if a browser frontend is added.
- `AreaType` allow-list is Application-level, not a DB check constraint.
- Overlap rejection for availability is Application-level.
- `ProviderCapability` DB index is non-unique; uniqueness of one active row per `(ProviderId, ServiceAttributeId)` is enforced in Application.
- Logo/media upload and portfolio APIs are not in this task.
- Task 7 unique indexes are **on live `MatchiDb`** via `20260909070502_Task07IndexDelta`. Historical `Task07ExecutionReviewIndexes` remains archived and was not applied.
- Task 8 Phase 2 (security/runtime) is complete. Task 8 Phase 3 / 8.2 is complete in code. `20260909083411_Task08IntegrityConcurrency` **was applied** to live `MatchiDb` in Phase 8.6.
- Task 8 Phase 4 / 8.3 Request cancel/delete vs Active Deal is complete in application code. Request remains `Open` after accept. Multiple Active Deals remain allowed.
- Task 8 Phase 5 / 8.4 catalog paging/`q` applied for Services/Businesses/Provider-by-serviceId. CORS and matching write-path deferred.
- Phase 8.7B: `Reviews.Id = 1` is Business-only (`BusinessId = 2`, `ProviderId = NULL`). It was the only dual-target row. Live `CK_Reviews_Target` is XOR via `20260909092717_Task08ReviewTargetXor` (applied).
- Intentionally deferred product/future: Request/Deal cancel-vs-accept race; BusinessProvider invite-must-accept; Provider geo/sort; CORS origins; Deal Complete/Cancel; ProductDelivery; rating aggregation; Matching persistence; sibling proposal auto-reject; pagination on non-catalog lists.
- Runtime HTTP/Swagger UI was **not** executed in Phases 2–5 (startup seeding is Development-gated and was not invoked against live `MatchiDb`).
- Existing JWTs issued before permission seed will lack Provider/Business permission claims until re-login.
- Local API start requires User Secrets or environment variables for `Jwt:Key` and `ConnectionStrings:DefaultConnection`. Previously committed credentials must be rotated.

---

## Verification

| Check | Result |
|---|---|
| `dotnet restore` | Succeeded |
| `dotnet build MatchiSolution.sln` | Succeeded, **0 warnings, 0 errors** |
| Swagger | Swashbuckle is referenced by `Matchi.Api`; the API project compiled. Swagger UI was **not** opened at runtime. |
| Runtime/API tests | **Not performed** |
| Database / `dotnet ef database update` | Baseline `Up()` never executed. `Task07IndexDelta`, `Task08IntegrityConcurrency`, and `Task08ReviewTargetXor` **were applied**. History: four rows. |
| Task 08 Phase 2 | Security/runtime hardening compiled. Seeder not executed against live DB. |
| Task 08 Phase 3 / 8.2 | Integrity/concurrency compiled. Migration created in that phase. |
| Task 08 Phase 4 / 8.3 | Request cancel/delete Active Deal guard compiled. No migration. Live schema/data unchanged. |
| Task 08 Phase 5 / 8.4 | Catalog paging/`q` + docs. No migration. Live schema/data unchanged. |
| Task 08 Phase 8.6 | Integrity index migration applied to live `MatchiDb`. Review XOR not changed in that phase. |
| Task 08 Phase 8.7A | READ-ONLY audit PASS. Review #1 = Business target. |
| Task 08 Phase 8.7B | Review #1 `ProviderId` nulled; XOR `CK_Reviews_Target` applied. Tests 67 passed. Build 0 errors (NU1900). No commit. No push. |
| Guid/long | Request/Provider/Business IDs remain `long`. |
| Legacy Loan/Question/Option/RequestAnswer | Not reintroduced in Application/API for this task. |
| Independent Provider | Create Provider does not require a Business. Removing membership does not delete Provider data. |
| Multiple Business memberships | Unique index is per `(BusinessId, ProviderId)` not per Provider. |
| Task 04 Matching | Application query + API compiled. Runtime HTTP not executed. |
| Task 05 Proposal | Application + API compiled. No migration. Runtime HTTP not executed. |
| Task 06 Deal | Accept creates Deal; GET list/detail customer-owned. No migration. Runtime HTTP not executed. |

---

## Seed changes (Task 03)

- Role `USER` is created if missing (required for OTP `EnsureRoleAsync`).
- Permissions listed above are created if missing and mapped to `ADMIN`; self-service set also mapped to `USER`.
- Existing plumbing providers + sample Business + two `BusinessProvider` rows are unchanged.
- No loan/question seed restored.
