import { DEFAULT_MAP_CENTER } from "../../../../shared/map/mapConstants";

export type ServiceAreaSelection = {
  lat: number;
  lng: number;
  radiusKm: number;
};

export { DEFAULT_MAP_CENTER };

export const DEFAULT_SERVICE_RADIUS_KM = 15;
export const MIN_SERVICE_RADIUS_KM = 5;
export const MAX_SERVICE_RADIUS_KM = 50;

export const RADIUS_AREA_TYPE = "Radius";

export function isValidServiceArea(value: ServiceAreaSelection | null): value is ServiceAreaSelection {
  if (value == null) {
    return false;
  }
  const { lat, lng, radiusKm } = value;
  return (
    Number.isFinite(lat) &&
    Number.isFinite(lng) &&
    lat >= -90 &&
    lat <= 90 &&
    lng >= -180 &&
    lng <= 180 &&
    Number.isFinite(radiusKm) &&
    radiusKm > 0
  );
}
