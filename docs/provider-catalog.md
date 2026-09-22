# Provider catalog offerings

A Provider profile (`Providers`) is not the same as what they offer.

```
User
 └── Provider (Providers.UserId)
      ├── ProviderServices → Services
      └── ProviderProducts → Products
```

`BusinessProviders` is membership in a Business. It is **not** required to offer catalog items or to appear in matching.

## Semantics

- Providers **select existing catalog rows**. They do not create Services or Products.
- `POST /api/providers/me/services` links `ServiceId` if the catalog service is active and not deleted.
- An existing non-deleted link cannot be added twice. A **soft-deleted** link is **reactivated**.
- `PUT` toggles `ProviderServices.IsActive`.
- `DELETE` soft-deletes the link.
- Products use `Price`, `IsAvailable`, `MinOrderQuantity`, `LeadTimeDays` on `ProviderProducts`.

Matching uses **service id** (not category). Offering “تعمیر کولر گازی” does not match “تعمیر پکیج”.

## APIs (existing, current user only)

```http
GET    /api/providers/me/services
POST   /api/providers/me/services          { "serviceId" }
PUT    /api/providers/me/services/{serviceId} { "isActive" }
DELETE /api/providers/me/services/{serviceId}

GET    /api/providers/me/products
POST   /api/providers/me/products          { "productId", "price?", "isAvailable?" }
PUT    /api/providers/me/products/{productId}
DELETE /api/providers/me/products/{productId}
```

Ownership is `Providers.UserId == JWT user`. The client must not send `providerId`.

Public catalog browse (for the picker): `GET /api/services/categories`, `GET /api/services?categoryId=`, `GET /api/products/categories`, `GET /api/products?categoryId=`.

## Frontend

`/provider/offerings` — «خدمات و محصولات من». Category → item → Add. Lists show Persian names, not ids.

Provider marketplace (`GET /api/provider/requests`) only includes Open requests whose service/product lines overlap the Provider’s active/available links.
