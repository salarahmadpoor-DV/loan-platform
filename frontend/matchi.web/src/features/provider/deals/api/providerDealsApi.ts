import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderDeal } from "./providerDealTypes";

export function getMyProviderDeals(): Promise<ProviderDeal[]> {
  return getJson<ProviderDeal[]>("/api/provider/deals");
}
