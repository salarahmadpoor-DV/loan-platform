# Matching

`GET /api/requests/{id}/matches` is a **computed ranking**, not a persisted Matching table. Proposal creation does not require a row from this endpoint; a Provider or Business owner can propose on an Open request independently.

```text
Customer
   ↓
Request
   ↓
RequestLocation (optional city/province/district + optional lat/lng)
   ↓
Existing filters (active party, service/product)
   ↓
Text area bonus (city / province / district)
   ↓
Geo overlay (Request point inside ProviderServiceArea / BusinessServiceArea radius)
   ↓
Nearest eligible candidate first (when distance exists)
   ↓
Cap 50
```

**Business membership is NOT required for Provider matching.** Independent Providers and Providers who also have BusinessProvider rows are both eligible. Matching queries `Providers` (and separately `Businesses`). It does not join `BusinessProviders` as a filter. A Provider is not duplicated because they belong to one or more businesses; Business is a separate candidate type with its own `BusinessServiceAreas`.

## Existing scores (unchanged)

| Signal | Points |
|---|---|
| Service | 50 |
| Product | 20 |
| Capability (Provider only) | 15 |
| Area | 10 |
| Availability | 5 |

Hard filters: candidate `Status = Active` and not deleted; service requests require a matching offered **service id** on `ProviderServices` / `BusinessServices` (`IsActive`, not deleted). Product-only requests require a matching `ProviderProducts` / `BusinessProducts` row (`IsAvailable`, not deleted). Category-only request lines (`ProductId` null) match an offered product in that category. Matching is **not** by ServiceCategory alone.

**Multiple lines:** a candidate qualifies if they offer **any** of the requested service ids (when a service is required) and, for product-only requests, **any** matching product/category. They do **not** need to cover every line. Hybrid requests set `RequireService = true` and `RequireProduct = false`: a service match is required; a product match only adds the product score.

Area is a **bonus**, not a hard filter.

Cancelled requests: the handler rejects matching (`ValidationException`). Max results: **50** (`MatchingScores.MaxResults`). Duplicates: one row per `(CandidateType, CandidateId)`.

## Geographic overlay

Radius on `ProviderServiceAreas.Radius` / `BusinessServiceAreas.Radius` is **kilometers** (same unit as the Provider map UX). Distance is Haversine (`GeoDistance.HaversineKm`, Earth radius 6371 km).

A service area is used for geo only when it is active, not deleted, has valid lat/lng, and `Radius > 0`. Incomplete request coordinates (only lat or only lng) are treated as **no point**; geo is skipped.

For each candidate with a request point:

1. Distances to usable areas are computed.
2. Areas that do not contain the point (`distanceKm > radiusKm`) are ignored.
3. The candidate's `distanceKm` is the **minimum containing** distance (one row even with many areas).
4. If contained, the Area score (+10) is applied when the text area bonus had not already applied.

Ranking when any distances exist:

```text
contained (distanceKm ASC) → others by existing score DESC
```

When the request has **no** valid coordinates, ranking is the previous score-only order.

City / province / district string matching is **kept**. Geo does not replace it.

## API

Same route. `MatchResultDto` adds optional `distanceKm` (JSON `distanceKm`). Older clients can ignore it.

## Limitations

- No SQL Server spatial / GIS index; Haversine runs in process after the existing candidate query, using that candidate set's service areas.
- Geo does not hide candidates who pass service/product filters but have no containing radius (they stay, without `distanceKm`, so text-only matches remain).
- Future: bounding-box or spatial SQL if scale requires it.

## Automatic Proposal / notification

Do **not** auto-create Proposals from this list. Matching is ranking for the customer UI. Proposal remains an explicit Provider/Business-owner action on an Open request.
