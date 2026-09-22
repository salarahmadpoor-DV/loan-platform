/** Default map viewport (Tehran). Not a stored user location. */
export const DEFAULT_MAP_CENTER = { lat: 35.6892, lng: 51.389 };

export const OSM_TILE_URL = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
export const OSM_ATTRIBUTION =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>';

export type MapPoint = {
  lat: number;
  lng: number;
};

export function isValidMapPoint(value: MapPoint | null): value is MapPoint {
  if (value == null) {
    return false;
  }
  const { lat, lng } = value;
  return (
    Number.isFinite(lat) &&
    Number.isFinite(lng) &&
    lat >= -90 &&
    lat <= 90 &&
    lng >= -180 &&
    lng <= 180
  );
}
