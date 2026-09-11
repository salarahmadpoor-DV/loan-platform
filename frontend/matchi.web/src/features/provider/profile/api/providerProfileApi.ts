import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderProfile } from "./providerProfileTypes";

export function getMyProviderProfile(): Promise<ProviderProfile> {
  return getJson<ProviderProfile>("/api/providers/me");
}
