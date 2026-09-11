import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderProposal } from "./providerProposalTypes";

export function getMyProviderProposals(): Promise<ProviderProposal[]> {
  return getJson<ProviderProposal[]>("/api/provider/proposals");
}
