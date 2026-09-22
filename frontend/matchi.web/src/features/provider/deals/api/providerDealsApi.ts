import { getJson, postJson } from "../../../../shared/api/httpClient";
import type { ProviderDeal } from "./providerDealTypes";

export function getMyProviderDeals(): Promise<ProviderDeal[]> {
  return getJson<ProviderDeal[]>("/api/provider/deals");
}

export function createDealExecution(dealId: number): Promise<{ executionId: number }> {
  return postJson(`/api/deals/${dealId}/executions`, {});
}
