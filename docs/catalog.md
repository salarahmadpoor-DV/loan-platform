# Matchi catalog

Matchi keeps **two separate catalogs**. There is no shared “catalog item” type.

## Service catalog

```
ServiceCategory
  → Service
    → ServiceAttribute (belongs to the Service)
      → ServiceAttributeOption
```

Customer request lines (`RequestServices`) point at a **Service**. `RequestServiceAttributes` must use attributes that belong to that same service.

## Product catalog

```
ProductCategory (optional ParentId)
  → Product
  → ProductAttribute (belongs to the ProductCategory, not the Product)
    → ProductAttributeOption
```

Customer request lines (`RequestProducts`) may send `productId`, `productCategoryId`, or both. Attributes on a product line must belong to the **resolved category**:

- If `productId` is set, the product’s `CategoryId` is the resolved category.
- If both ids are set, the product must belong to that category.
- If only `productCategoryId` is set, that category is used.

## Stable identifiers

Do not hard-code database identity values in application code. Resolve rows by:

| Record | Stable key |
|---|---|
| ServiceCategory / ProductCategory | `Slug` (ASCII, unique) |
| Service / Product | `Slug` (ASCII, unique among non-deleted rows) |
| ServiceAttribute / ProductAttribute | `Code` unique per parent |
| Options | `Value` unique per attribute |

Customer-facing `Name` / `DisplayName` values are Persian. Slugs and codes stay English.

## Seed strategy

`DatabaseSeeder` (Development only, when `Seed:Enabled` is true) calls `CatalogSeeder`.

`CatalogSeeder` is **idempotent**:

1. Find by slug/code/value.
2. Create if missing.
3. If a matching row exists with different name/parent/data type, **log a warning** and leave the row unchanged.
4. Never delete catalog rows. User-created rows (other slugs) are preserved.

It is safe to run on development, test, and staging databases. Production is **not** seeded unless the host is Development **and** `Seed:Enabled` is true (same rule as the rest of `DatabaseSeeder`).

### How to seed

Set in API configuration (typically `appsettings.Development.json`):

```json
"Seed": { "Enabled": true }
```

Start the API in the Development environment. The seeder runs once at startup.

### How to add a Service

1. Add a `ServiceSeed` under the correct `ServiceCategorySeed` in `Matchi.Infrastructure/Persistence/Seed/CatalogBlueprint.cs`.
2. Use a new ASCII slug and optional attributes/options.
3. Restart the API (or call `CatalogSeeder.SeedAsync`) so the row is inserted.

### How to add a Product

1. Add a `ProductSeed` under the correct `ProductCategorySeed` (leaf categories are preferred).
2. Category-level attributes live on the `ProductCategorySeed`, not on the product.
3. Re-run the seeder.

## Request validation

FluentValidation (`RequestWriteRules`) remains the authority:

- Product missing / inactive / deleted → `The selected product was not found or is inactive.`
- Product + category mismatch → `The selected product does not belong to the specified product category.`
- Category missing / inactive / deleted → `The selected product category was not found or is inactive.`
- Attribute not on the resolved category, or inactive/deleted → `Product attribute does not belong to the product category for this line.`

The older combined message `Product, product category, or product attribute is invalid for this line.` is no longer emitted; the rules above replaced that single check.

Service lines still require an active service; service attributes must belong to that service.

## APIs used by the request wizard

No duplicate catalog endpoints. Public (no auth):

```http
GET /api/services/categories
GET /api/services?categoryId=&q=&page=&pageSize=
GET /api/services/{serviceId}
GET /api/products/categories
GET /api/products?categoryId=&q=&page=&pageSize=
GET /api/products/{productId}
GET /api/products/categories/{categoryId}/attributes
```

Service detail now includes `attributes` (id, name, code, dataType, isRequired, displayOrder, options) in addition to `attributeCount`.

The SPA request wizard loads these lists and submits live ids on `POST /api/requests`. It does not type raw catalog ids.

## Provider and Business offerings

Catalog rows are shared. Offerings are join tables:

- [Provider catalog](./provider-catalog.md) — `ProviderServices` / `ProviderProducts`
- [Business catalog](./business-catalog.md) — `BusinessServices` / `BusinessProducts`

Matching uses those joins (exact `ServiceId` / `ProductId`, plus category-only product lines). See [matching.md](./matching.md).

## Troubleshooting

**`Product, product category, or product attribute is invalid for this line.`** (or the newer split messages)

Typical causes:

1. Frontend sent a product id that does not exist in this database (typed/stale id).
2. `productId` and `productCategoryId` disagree.
3. `productAttributeId` belongs to another category (attributes are per **category**).
4. Product, category, or attribute is inactive or soft-deleted.
5. Catalog was never seeded (`Seed:Enabled` / Development).

Fix: pick catalog rows from the APIs above, send the product’s real `categoryId`, and only send attributes returned by `GET /api/products/categories/{id}/attributes`.
