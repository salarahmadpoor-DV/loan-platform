# Business catalog offerings

Business ownership is `Businesses.OwnerUserId`. There is no BusinessOwner role and no extra registration.

```
User
 └── owned Business (OwnerUserId)
      ├── BusinessServices → Services
      ├── BusinessProducts → Products
      └── BusinessProviders (membership of Providers — not offerings)
```

## Semantics

- Owners assign **existing** catalog Services/Products.
- Service offer fields: `IsActive`, `CanCustomerChooseProvider`, `MinPrice`, `MaxPrice`.
- Product offer fields: `Price`, `IsAvailable`, `MinOrderQuantity`, `LeadTimeDays`.
- Duplicate active links are rejected; soft-deleted links are reactivated.
- Inactive/deleted catalog rows cannot be added.
- Non-owners receive not found (same as other `/api/businesses/me/*` operations).

Provider assignment to a job is **not** configured here. `BusinessProviders` remains membership.

## APIs (existing)

All `/me` routes resolve businesses with `OwnerUserId == current user`. Optional `businessId` is required when the user owns more than one Business.

```http
GET    /api/businesses/me/services?businessId=
POST   /api/businesses/me/services
PUT    /api/businesses/me/services/{serviceId}
DELETE /api/businesses/me/services/{serviceId}?businessId=

GET    /api/businesses/me/products?businessId=
POST   /api/businesses/me/products
PUT    /api/businesses/me/products/{productId}
DELETE /api/businesses/me/products/{productId}?businessId=
```

## Frontend

`/business/catalog` and `/provider/business/catalog` — same owner UI. Catalog pickers use public service/product APIs.

There is **no** Business request-inbox page. Customer matching still ranks Business candidates via `BusinessServices` / `BusinessProducts`. Owners propose with `proposerType: Business`.
