import { getJson, postJson } from "../../../../shared/api/httpClient";
import type {
  AcceptProposalResult,
  ProposalDetail,
  ProposalListItem,
} from "./proposalTypes";

export function getRequestProposals(requestId: number): Promise<ProposalListItem[]> {
  return getJson<ProposalListItem[]>(`/api/requests/${requestId}/proposals`);
}

export function getProposalById(proposalId: number): Promise<ProposalDetail> {
  return getJson<ProposalDetail>(`/api/proposals/${proposalId}`);
}

export function acceptProposal(proposalId: number): Promise<AcceptProposalResult> {
  return postJson<AcceptProposalResult>(`/api/proposals/${proposalId}/accept`);
}
