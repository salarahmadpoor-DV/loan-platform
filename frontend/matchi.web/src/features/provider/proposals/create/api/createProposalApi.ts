import { postJson } from "../../../../../shared/api/httpClient";
import type {
  CreateProviderProposalBody,
  CreateProviderProposalResult,
} from "./createProposalTypes";

export function createProviderProposal(
  requestId: number,
  body: CreateProviderProposalBody,
): Promise<CreateProviderProposalResult> {
  return postJson<CreateProviderProposalResult, CreateProviderProposalBody>(
    `/api/requests/${requestId}/proposals`,
    body,
  );
}
