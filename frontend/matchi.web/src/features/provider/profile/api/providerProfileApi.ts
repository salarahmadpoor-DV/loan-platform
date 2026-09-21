import { getJson, postJson } from "../../../../shared/api/httpClient";
import type { ProviderProfile } from "./providerProfileTypes";

export type CreateProviderBody = {
  name: string;
  description?: string;
  lat?: number;
  lng?: number;
};

export type CreateProviderResult = {
  providerId: number;
};

export function getMyProviderProfile(): Promise<ProviderProfile> {
  return getJson<ProviderProfile>("/api/providers/me");
}

export function createMyProvider(body: CreateProviderBody): Promise<CreateProviderResult> {
  return postJson<CreateProviderResult, CreateProviderBody>("/api/providers", body);
}
