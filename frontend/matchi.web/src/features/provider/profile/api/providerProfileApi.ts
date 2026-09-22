import { getJson, postJson, putJson } from "../../../../shared/api/httpClient";
import type { ProviderProfile, ProviderServiceArea } from "./providerProfileTypes";

export type CreateProviderBody = {
  name: string;
  description?: string;
  lat?: number;
  lng?: number;
};

export type CreateProviderResult = {
  providerId: number;
};

export type UpdateMyProviderBody = {
  name: string;
  description?: string | null;
  mobile?: string | null;
  lat?: number | null;
  lng?: number | null;
};

export type ProviderServiceAreaBody = {
  areaType: string;
  province?: string | null;
  city?: string | null;
  district?: string | null;
  lat?: number | null;
  lng?: number | null;
  radius?: number | null;
  isActive?: boolean;
};

export function getMyProviderProfile(): Promise<ProviderProfile> {
  return getJson<ProviderProfile>("/api/providers/me");
}

export function createMyProvider(body: CreateProviderBody): Promise<CreateProviderResult> {
  return postJson<CreateProviderResult, CreateProviderBody>("/api/providers", body);
}

export function updateMyProvider(body: UpdateMyProviderBody): Promise<{ success: boolean }> {
  return putJson<{ success: boolean }, UpdateMyProviderBody>("/api/providers/me", body);
}

export function getMyProviderServiceAreas(): Promise<ProviderServiceArea[]> {
  return getJson<ProviderServiceArea[]>("/api/providers/me/areas");
}

export function addMyProviderServiceArea(
  body: ProviderServiceAreaBody,
): Promise<{ id: number }> {
  return postJson<{ id: number }, ProviderServiceAreaBody>("/api/providers/me/areas", body);
}

export function updateMyProviderServiceArea(
  areaId: number,
  body: ProviderServiceAreaBody,
): Promise<{ areaId: number; success: boolean }> {
  return putJson<{ areaId: number; success: boolean }, ProviderServiceAreaBody>(
    `/api/providers/me/areas/${areaId}`,
    body,
  );
}
