import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderRequestInboxItem } from "./providerRequestTypes";

export function getProviderRequestInbox(): Promise<ProviderRequestInboxItem[]> {
  return getJson<ProviderRequestInboxItem[]>("/api/provider/requests");
}
