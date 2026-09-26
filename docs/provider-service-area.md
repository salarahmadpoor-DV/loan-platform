# Provider Service Area

The Provider working area is chosen on a map. The map is a UX layer over the existing Provider geographic fields. It does not introduce a new location model or matching algorithm.

## User flow

```text
Provider Profile or Onboard
      ↓
Service Area map
      ↓
Click/tap to set center
      ↓
Adjust radius with the slider
      ↓
Confirm / save
      ↓
Existing Provider APIs persist lat, lng, radius
```

The user does not type latitude, longitude, or radius.

A saved Radius area is loaded back onto the map (marker + circle) so it can be edited.

## Domain storage (unchanged)

Two existing stores remain in use:

| Store | Fields | API |
|---|---|---|
| `Providers` profile point | `Lat`, `Lng` | `POST /api/providers` (create), `PUT /api/providers/me` (update) |
| `ProviderServiceAreas` | `AreaType = Radius`, `Lat`, `Lng`, `Radius` | `GET/POST /api/providers/me/areas`, `PUT /api/providers/me/areas/{areaId}` |

Radius-type areas already require `lat`, `lng`, and `radius > 0` (`CatalogRules.EnsureAreaPayload`). The SPA sends those values after map selection.

`Radius` is treated as **kilometers** in the UI. Leaflet draws the circle in meters (`radiusKm * 1000`). Matching uses city/province/district as an Area score bonus and, when the Request has lat/lng, Haversine containment of that point in this radius. See [matching.md](./matching.md).

## Map library

There was no map dependency in the SPA. Version 1 uses:

- **Leaflet** `1.9` (OpenStreetMap raster tiles)
- **react-leaflet** `5`

Why:

- Works with React
- Markers and circles without a paid SDK
- No API key in frontend source
- Touch pan/zoom on mobile
- Surrounding controls stay MUI/RTL; the map pane is `dir="ltr"` so tiles stay geographically standard

Default viewport when nothing is saved: Tehran (`35.6892`, `51.389`). That is only a starting camera, not a stored location. Browser geolocation is **not** requested.

## Geocoding / address search

The repository has no geocoding client or API key. Version 1 does **not** add Nominatim, Google Places, or another search service.

Users pick the center by clicking the map. A city/address search box can be added later if a service is chosen and configured without committing secrets.

If an external geocoder is added later, document:

- Service name
- Purpose (forward geocode of the search box)
- Whether an API key is required
- Env var name (never commit the secret)

## Frontend surfaces

- `ServiceAreaMapPicker` — map + radius slider
- Provider onboard (`/provider/onboard`) — map instead of lat/lng text fields; create still uses `POST /api/providers`; a Radius area is posted to `/api/providers/me/areas` when a center was selected
- Provider profile (`/provider/profile`) — loads existing Radius area or profile lat/lng; Confirm calls PUT/POST areas and `PUT /api/providers/me`

## Validation

Frontend: a center must exist and `radius > 0` before Confirm on the profile map.

Backend (unchanged): Radius areas require lat, lng, and radius greater than zero.

Customer Request location (a point, not a coverage radius) is documented in [request-location.md](./request-location.md).
