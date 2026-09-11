import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getProposalById } from "../api/proposalsApi";

export function useProposal(proposalId: number | undefined) {
  return useQuery({
    queryKey:
      proposalId != null
        ? queryKeys.proposals.detail(proposalId)
        : queryKeys.proposals.all,
    queryFn: () => getProposalById(proposalId as number),
    enabled: proposalId != null && Number.isFinite(proposalId) && proposalId > 0,
  });
}
