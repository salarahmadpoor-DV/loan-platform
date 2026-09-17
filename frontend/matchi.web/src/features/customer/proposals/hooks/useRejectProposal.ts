import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { rejectProposal } from "../api/proposalsApi";

export function useRejectProposal() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (proposalId: number) => rejectProposal(proposalId),
    onSuccess: (result) => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.proposals.all });
      void queryClient.invalidateQueries({
        queryKey: queryKeys.proposals.detail(result.proposalId),
      });
      void queryClient.invalidateQueries({ queryKey: queryKeys.provider.proposals() });
    },
  });
}
