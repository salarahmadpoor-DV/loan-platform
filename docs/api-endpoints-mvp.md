# API Endpoints — MVP

**Date:** 2026-09-11  
**Status:** Synchronized with the codebase after Task 10 (Provider marketplace reads)

Legend:

| Status | Meaning |
|---|---|
| **Implemented** | Controller + handler exist and are the current contract |
| **Stub** | Endpoint exists but does not persist/complete the real workflow |
| **Target** | Planned MVP contract. Domain exists; Application/API not implemented |

Auth: JWT Bearer unless noted. OTP login issues the token.

IDs are `long` (not `Guid`). Service intake uses `ServiceAttribute`, not Question/Option.

---

## Marketplace flow (target)

```text
Customer
  → Request
  → Matching (Provider XOR Business)
  → Proposal (exactly one of BusinessId / ProviderId)
  → Deal
  → ServiceExecution / ProductDelivery
  → Review (Deal + Customer; exactly one of Business or Provider)
```

---

## 1. Implemented

### Health

```http
GET /api/health
```

No auth.

Response:

```json
{ "status": "Healthy" }
```

---

### Auth

```http
POST /api/auth/send-otp
POST /api/auth/verify-otp
```

No auth.

**send-otp**

```json
{ "mobile": "0912..." }
```

`202 Accepted`: `{ "requestId": "..." }`

OTP is generated per request, stored in memory, expires after a short TTL, and is limited by failed attempts. The API returns only `requestId`. The OTP is not a fixed code.

**verify-otp**

```json
{ "mobile": "0912...", "otp": "<issued-otp>", "requestId": "..." }
```

`200 OK`:

```json
{
  "accessToken": "...",
  "refreshToken": "...",
  "expiresAtUtc": "...",
  "user": { "id": 1, "mobile": "0912...", "roles": ["ADMIN"] }
}
```

---

### Users

```http
GET /api/users/me
PUT /api/users/me
```

Bearer required.

**GET me**

```json
{ "id": 1, "mobile": "0912...", "name": "..." }
```

**PUT me**

```json
{
  "name": "...",
  "location": { "lat": 35.8, "lng": 51.0, "address": "..." }
}
```

`location` is accepted on the API; the current handler only persists `name`.

---

### Services

```http
GET /api/services/categories
GET /api/services?categoryId=&q=&page=&pageSize=
GET /api/services/{serviceId}
```

No auth.

Categories: `{ "id", "name" }`

Services list: `{ "page", "pageSize", "items": [ { "id", "name", "categoryId" } ] }`

`q` filters by service name (`Contains`). Results are ordered by `DisplayOrder`, then `Id`. `page` defaults to 1; `pageSize` defaults to 20 and is capped at 100. Filtering is applied before skip/take.

Service detail: `{ "id", "name", "categoryId", "attributeCount" }`

Attributes themselves are not returned yet.

---

### Providers

```http
POST /api/providers
GET  /api/providers/{providerId}
GET  /api/providers?serviceId=&lat=&lng=&radiusKm=&sort=&page=&pageSize=
```

Create requires Bearer. Reads are public.

**POST body** (`CreateProviderCommand`)

```json
{ "name": "...", "lat": 35.83, "lng": 50.99 }
```

Creates a provider for the current user. `201` `{ "providerId" }`.

**GET one / search item**

```json
{
  "id": 1,
  "name": "...",
  "rating": 0,
  "isActive": true,
  "lat": 35.83,
  "lng": 50.99
}
```

`isActive` is derived from `Status == "Active"`. Search requires `serviceId` (otherwise `items` is empty). Pagination (`page` / `pageSize`, max 100) is applied in the query after ordering by `Id`. `lat` / `lng` / `radiusKm` / `sort` are accepted but not applied.

Provider catalog APIs (services, products, capabilities, areas, availability) are **Target**.

---

### Businesses

```http
POST /api/businesses
GET  /api/businesses?page=&pageSize=
POST /api/businesses/{businessId}/invite-provider
PUT  /api/businesses/business-providers/{businessProviderId}/accept
```

Create / invite / accept require Bearer. List is public.

**POST create**

```json
{
  "name": "...",
  "address": "...",
  "location": { "lat": 35.83, "lng": 50.99, "address": "..." },
  "ownerContact": "..."
}
```

`ownerContact` is ignored. Owner is the current user.

**GET list item:** `{ "id", "name", "address" }` — public list of non-deleted businesses, ordered by `Id`, paginated (`page` / `pageSize`, max 100).

**Invite**

```json
{ "providerId": 1, "role": "Technician" }
```

`202` `{ "inviteId", "status": "Pending" }` — persists membership as `Pending` (owner-checked).

**Accept:** `200` `{ "businessProviderId", "status": "Active" }` — Provider user activates their membership (`PROVIDER_EDIT`).

Owner `POST /api/businesses/me/providers` can also create membership (`Active` by default). This is owner membership management, not an anonymous invite bypass.

Business catalog APIs (services, products, areas, availability) are **Target**.

---

### Requests

```http
POST   /api/requests
GET    /api/requests/me
GET    /api/requests/{requestId}
GET    /api/requests/{requestId}/matches
GET    /api/requests/{requestId}/proposals
POST   /api/requests/{requestId}/proposals
PUT    /api/requests/{requestId}
POST   /api/requests/{requestId}/cancel
DELETE /api/requests/{requestId}
```

Bearer required. Customer is resolved from the current user (`Customer` is created if needed). `CustomerId` is not accepted from the client. The Request is customer-owned; it has no `ProviderId` or `BusinessId`.

Owner-only for get / update / cancel / delete. A request that is not owned by the current user is returned as `404`. Permission `REQUEST_VIEW` is **not** required (it is an admin seed permission).

`requestType`: `Service` | `Product` | `Hybrid`.

- Service: at least one service, no products
- Product: at least one product, no services
- Hybrid: at least one service and at least one product

Quantity must be greater than zero. A product line must reference `productId` and/or `productCategoryId`. If both are sent, the category must match the product. Service attributes must belong to the selected service. Product attributes must belong to the resolved product category. If both `timeFrom` and `timeTo` are set, `timeFrom` must be earlier than `timeTo`.

Create and update use the same body. `PUT` replaces services, products, attributes, location, and schedule. Only an `Open` request can be updated or cancelled.

**POST / PUT body**

```json
{
  "requestType": "Hybrid",
  "title": "تعمیر کولر",
  "description": "کولر روشن نمی‌شود",
  "services": [
    {
      "serviceId": 10,
      "quantity": 1,
      "description": null,
      "displayOrder": 0,
      "attributes": [
        { "serviceAttributeId": 101, "value": "LG" }
      ]
    }
  ],
  "products": [
    {
      "productId": 1,
      "productCategoryId": null,
      "quantity": 1,
      "unit": null,
      "description": null,
      "displayOrder": 0,
      "attributes": [
        { "productAttributeId": 201, "value": "..." }
      ]
    }
  ],
  "location": {
    "province": null,
    "city": "تهران",
    "district": "پونک",
    "address": "...",
    "lat": 35.7,
    "lng": 51.3
  },
  "schedule": {
    "date": "2026-09-10",
    "timeFrom": null,
    "timeTo": null,
    "isFlexible": true
  }
}
```

Location and schedule are optional. Domain can store multiple locations/schedules; this API persists **one** of each.

`201` create: `{ "requestId" }`.  
`PUT`: `{ "requestId", "success": true }`.  
`POST .../cancel` sets `Status` to `Cancelled` (row remains readable): `{ "requestId", "status": "Cancelled" }`.
If the Request has a non-deleted Deal with `Status = Active`, cancel returns **409 Conflict** and does not change the Request or Deal.
`DELETE` soft-deletes (`IsDeleted`); `204`.
If the Request has a non-deleted Active Deal, delete returns **409 Conflict** and does not change the Request or Deal.
Non-owners receive **404** for cancel/delete (same as get); Active Deal existence is not queried until ownership is established.

**GET one / GET me item**

```json
{
  "id": 1,
  "customerId": 1,
  "requestType": "Hybrid",
  "title": "...",
  "description": "...",
  "status": "Open",
  "services": [
    {
      "id": 1,
      "serviceId": 10,
      "quantity": 1,
      "description": null,
      "displayOrder": 0,
      "attributes": [
        { "serviceAttributeId": 101, "value": "LG" }
      ]
    }
  ],
  "products": [],
  "location": {
    "province": null,
    "city": "تهران",
    "district": "پونک",
    "address": "...",
    "lat": 35.7,
    "lng": 51.3
  },
  "schedule": {
    "date": "2026-09-10",
    "timeFrom": null,
    "timeTo": null,
    "isFlexible": true
  },
  "createDate": "2026-09-08T10:00:00Z",
  "updateDate": null
}
```

`GET /api/requests/me` returns an array of that shape (not deleted). Cancelled requests are included; deleted ones are not.

Attributes are `ServiceAttribute` / `ProductAttribute` values, not Question/Option.

Removed (do not restore): `POST /api/requests` ServiceRequest + `RequestAnswerDto` / `questionId` / `optionId`.

Matching is `GET /api/requests/{requestId}/matches` (see Matching below). Proposal create/list are nested under Request. Deal/chat/complaint/payment are **not** part of Request CRUD.

---

### Reviews (read)

```http
GET /api/providers/{providerId}/reviews
GET /api/businesses/{businessId}/reviews
```

No auth. Non-deleted reviews for that Provider or Business. A review with both targets (legacy data) can appear in both lists; `targetType` follows the list being queried. Deleted reviews are excluded.

Create-review is implemented: `POST /api/deals/{dealId}/reviews` (Bearer, Deal customer). Target is XOR: exactly one of `businessId` / `providerId`. The old `POST /api/introductions/{introductionId}/reviews` was removed.

---

### Matching

```http
GET /api/requests/{requestId}/matches
```

Bearer. Owner-only (`404` if missing or not owned). Does not mutate the Request. Empty candidate list is `200` `[]`.

```json
{
  "candidateType": "Provider",
  "candidateId": 10,
  "displayName": "...",
  "score": 50,
  "rank": 1
}
```

`candidateType`: `Provider` | `Business`. Scores: service +50, product +20, capability +15 (Provider), area +10, availability +5. Max 50. `BusinessProvider` is not required for Business candidates.

Removed: Guid matching and `POST /api/requests/{requestId}/match` (GET-only).

---

### Proposals

```http
POST /api/requests/{requestId}/proposals
GET  /api/requests/{requestId}/proposals
GET  /api/proposals/{proposalId}
POST /api/proposals/{proposalId}/accept
POST /api/proposals/{proposalId}/reject
```

Bearer (`[Authorize]`). No Proposal-specific permission. Identity is resolved from the current user; do not send `providerId`, `userId`, `ownerUserId`, or `customerId`. Appearing in matching does **not** grant create rights.

**POST create** — `RequestId` from the route. Body:

```json
{
  "proposerType": "Provider",
  "businessId": null,
  "totalPrice": 1500000,
  "deliveryFee": 0,
  "message": "...",
  "proposedDate": "2026-09-12",
  "proposedTimeFrom": "09:00:00",
  "proposedTimeTo": "11:00:00",
  "expireAt": "2026-09-15T12:00:00Z",
  "items": [
    {
      "itemType": "Service",
      "serviceId": 10,
      "productId": null,
      "description": null,
      "quantity": 1,
      "unitPrice": 1500000,
      "totalPrice": 1500000,
      "displayOrder": 0
    }
  ]
}
```

`proposerType`: `Provider` | `Business`. Provider identity is `Provider.UserId`. Business identity is `Business.OwnerUserId`. `businessId` is optional and only used to choose among businesses **owned** by the current user. Membership (`BusinessProvider`) is not used. Request must exist, not be deleted, and be `Open` (`400` otherwise). Party XOR and item Product XOR Service are enforced. Catalog items must be active and offered by that Provider/Business. Status starts as `Pending`. `201` `{ "proposalId" }`. Duplicate proposals are allowed. `ExpireAt` is stored; it is not processed automatically.

**GET list** — Request owner only (`404` if missing or not owned). Summaries: `id`, `requestId`, `proposerType`, `proposerId`, `totalPrice`, `deliveryFee`, `status`, `expireAt`, `createDate`.

**GET one** — Request owner of the Proposal’s Request (`404` if not owned / missing). Includes items.

**Accept / Reject** — Request owner. Request must be `Open` to accept. Only `Pending` → `Accepted` or `Pending` → `Rejected` (`400` otherwise). Sibling proposals stay `Pending`. Request status is unchanged.

**Accept** creates a Deal in the same save:

```json
{
  "proposalId": 123,
  "status": "Accepted",
  "dealId": 456
}
```

Reject does not create a Deal. There is no `POST /api/deals`.

---

### Provider marketplace (workspace)

```http
GET /api/provider/requests
GET /api/provider/proposals
GET /api/provider/deals
GET /api/provider/executions
```

Bearer. Policy `ProviderWorkspace` = authenticated + JWT role `PROVIDER` (not permission claims). `USER` without `PROVIDER` → **403**. Missing Provider profile → **404**. Empty lists are `200 []`.

Does **not** replace customer matching (`GET /api/requests/{id}/matches`). Inbox uses the same eligibility rules as Provider matching (open requests only; Business candidates are not included).

**GET `/api/provider/requests`** — eligible open requests only (not all customer requests).

```json
{
  "requestId": 10,
  "requestType": "Service",
  "serviceSummary": "AC repair",
  "categorySummary": "Home",
  "location": { "province": null, "city": "تهران", "district": "پونک" },
  "createdDate": "2026-09-11T12:00:00Z",
  "status": "Open"
}
```

**GET `/api/provider/proposals`** — `Proposal.ProviderId` of the current Provider only (Pending/Accepted/Rejected). `dealId` is null unless a non-deleted Deal exists.

**GET `/api/provider/deals`** — visible when `Proposal.ProviderId` is the current Provider **or** an `Assigned` execution assignment exists for that Provider. `BusinessProvider` membership is **not** a grant.

**GET `/api/provider/executions`** — proposal-party Provider **or** `Assigned` assignment (including primary executor). Independent Providers have no assignments; they still see executions on their own deals.

---

### Deals

```http
GET /api/deals
GET /api/deals/{dealId}
```

Bearer. Customer-owned only (`Request.Customer.UserId`). No `customerId` / `providerId` / `businessId` query filters. Non-owned → `404`. Party is on the Proposal, not duplicated onto Deal. `BusinessProvider` is not used.

**GET list item**

```json
{
  "id": 456,
  "requestId": 10,
  "proposalId": 123,
  "status": "Active",
  "totalPrice": 1500000,
  "acceptedAt": "2026-09-08T12:00:00Z"
}
```

**GET one** also includes `customerId`, `createDate`, `updateDate`.

Deal cancel / complete / execution / delivery are **not** implemented.

---

## 2. Target (not implemented)

These are the MVP contracts. Domain entities exist. Do not treat them as live API.

### Execution / delivery / deal cancel

```http
POST /api/deals/{dealId}/cancel
GET  /api/deals/{dealId}/executions
POST /api/executions/{executionId}/assignments
GET  /api/deals/{dealId}/deliveries
```

---

### Reviews (create)

```http
POST /api/deals/{dealId}/reviews
```

Bearer. Customer of the Deal. Body includes `rating` (1–5), optional `comment`, and **exactly one** of `businessId` / `providerId`. Duplicate active reviews for the same target return **409** on unique-index race (pre-check remains **400**).

---
### Catalog (provider / business capabilities)

```http
PUT /api/providers/{providerId}/services
PUT /api/providers/{providerId}/products
PUT /api/providers/{providerId}/capabilities
PUT /api/providers/{providerId}/areas
PUT /api/providers/{providerId}/availabilities
PUT /api/businesses/{businessId}/services
PUT /api/businesses/{businessId}/products
PUT /api/businesses/{businessId}/areas
PUT /api/businesses/{businessId}/availabilities
```

---

### Communication / trust (later MVP+)

```http
GET  /api/requests/{requestId}/conversation
POST /api/conversations/{conversationId}/messages
POST /api/requests/{requestId}/complaints
POST /api/requests/{requestId}/cancellations
```

Media, verification, and trust-score APIs are out of MVP unless explicitly pulled in.

---

## 3. Removed (do not document as current)

| Removed | Replacement |
|---|---|
| `POST /api/loanrequests` | `POST /api/requests` |
| `POST/GET/PUT /api/introductions...` | Proposal |
| `POST /api/requests` ServiceRequest + answers | `POST /api/requests` (ServiceAttribute / ProductAttribute) |
| `GET /api/requests/{id}` ServiceRequestDto | `GET /api/requests/{requestId}` |
| `GET /api/requests/user/{userId}` | `GET /api/requests/me` |
| Guid matching | `long` request matching |
| `POST /api/introductions/{id}/reviews` | `POST /api/deals/{dealId}/reviews` |

---

## 4. Auth and errors

- Missing/invalid JWT → `401`
- Permission `REQUEST_VIEW` is seeded for `ADMIN`; Request CRUD uses `[Authorize]` plus owner checks (non-owners → `404`)
- FluentValidation → `400` problem details
- `KeyNotFoundException` → `404`
- `ConflictException` → `409` (unique/concurrency conflicts; Request cancel/delete while an Active Deal exists)
- Other unhandled → `500` (production detail is sanitized; `traceId` is always present)

JWT settings: `Jwt:Key` (min 32 chars), `Jwt:Issuer`, `Jwt:Audience`. Values must come from User Secrets or environment variables; committed `appsettings.json` keys are empty.

CORS is not configured in the API. Origins are a deployment concern; do not enable `AllowAnyOrigin` with credentials.

---

## 5. Notes for implementers

- Domain already has Request, Proposal (XOR party), Deal, execution, delivery, Review.
- Task 02 implemented Request CRUD. Task 04 implemented matching (read-only). Task 05 implemented Proposal. Task 06 implemented Accept → Deal. Task 07 implemented execution and reviews.
- Public catalog lists that wrap `{ page, pageSize, items }` apply skip/take (`GET /api/services`, `GET /api/businesses`, `GET /api/providers` with `serviceId`). Provider `lat`/`lng`/`radiusKm`/`sort` remain unused.
- Invite/accept business membership persist; owner `/me/providers` can set Active membership.
- Seed: admin + demo user `09120000000`, plumbing category/service, sample providers and one business. No loan category. Seed runs only when `Seed:Enabled` is true **and** the environment is Development.
