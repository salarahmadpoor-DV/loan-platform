import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { acceptProposal } from "../api/proposalsApi";

export function useAcceptProposal() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (proposalId: number) => acceptProposal(proposalId),
    onSuccess: (result) => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.proposals.all });
      void queryClient.invalidateQueries({
        queryKey: queryKeys.proposals.detail(result.proposalId),
      });
      void queryClient.invalidateQueries({ queryKey: queryKeys.requests.all });
      void queryClient.invalidateQueries({ queryKey: queryKeys.deals.all });
    },
  });
}
