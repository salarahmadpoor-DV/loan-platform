import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderBusinessMembership } from "./providerMembershipTypes";

export function getMyProviderMemberships(): Promise<ProviderBusinessMembership[]> {
  return getJson<ProviderBusinessMembership[]>("/api/providers/me/businesses");
}
