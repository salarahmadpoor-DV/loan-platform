import { getJson } from "../../shared/api/httpClient";
import type {
  LocationCity,
  LocationDistrict,
  LocationProvince,
  LocationResolveResponse,
} from "./types";

export function getProvinces(): Promise<LocationProvince[]> {
  return getJson<LocationProvince[]>("/api/locations/provinces");
}

export function getCitiesByProvince(provinceId: number): Promise<LocationCity[]> {
  return getJson<LocationCity[]>(`/api/locations/provinces/${provinceId}/cities`);
}

export function getDistrictsByCity(cityId: number): Promise<LocationDistrict[]> {
  return getJson<LocationDistrict[]>(`/api/locations/cities/${cityId}/districts`);
}

export function resolveLocation(lat: number, lng: number): Promise<LocationResolveResponse> {
  return getJson<LocationResolveResponse>("/api/locations/resolve", {
    params: { lat, lng },
  });
}
