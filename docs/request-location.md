# Request location

Customer Request location is where the service should happen. It is not a Provider service area.

| Concept | Store | Meaning |
|---|---|---|
| Request location | `RequestLocations` | Customer need: point + optional city/province/district/address |
| Provider coverage | `ProviderServiceAreas` | Where a Provider can work (including Radius lat/lng/radius) |

Do not reuse `ProviderServiceAreas` for customer requests.

## Location model (existing)

`RequestLocation` already has:

- `Address`, `Province`, `City`, `District`
- `Lat`, `Lng`

No new columns or endpoints were added.

`POST /api/requests` and `PUT /api/requests/{id}` already accept optional `location`:

```json
{
  "province": "تهران",
  "city": "تهران",
  "district": null,
  "address": null,
  "lat": 35.7,
  "lng": 51.4
}
```

Location is **optional**. If the customer neither picks a map point nor fills place text, the SPA omits `location`. Backend validation only limits string lengths when `location` is present; coordinates are not required.

There is no dedicated Request edit screen in the SPA. `PUT` remains available on the API for Open requests.

## Map UX

The create wizard step «موقعیت» uses the same Leaflet + OpenStreetMap stack as Provider service area (`shared/map`).

- Customer: click/tap a **point** (marker only, no radius)
- Provider: center + radius circle (separate picker)

The user does not type latitude/longitude. Browser geolocation is not requested. Default camera is Tehran if nothing is selected.

City / province / district / address remain optional text fields because **matching currently uses those strings**, not coordinates.

## Matching

See [matching.md](./matching.md).

`GetRequestMatchesQuery` still maps `RequestLocation.City` / `Province` / `District` for the Area score. When both `Lat` and `Lng` are present, Haversine containment against `ProviderServiceAreas` (and `BusinessServiceAreas`) adds `distanceKm` and ranks nearer contained candidates first.

**Map coordinates are used for precise location and geographic ranking. City/province/district matching is still applied. Geographic-distance matching does not require Business membership.**

## Persistence path

```text
Create Request map click
      ↓
lat, lng (+ optional city/province/district/address)
      ↓
POST /api/requests { location }
      ↓
RequestLocation row
```

GET request detail shows place text and a read-only map when lat/lng exist.

## Limitations

- No address geocoding/search (same as Provider map)
- No Request edit form in the SPA
- Product-category id still lives on the same wizard step as location (existing wizard layout)
