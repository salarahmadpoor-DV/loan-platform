import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../../shared/api/queryKeys";
import { createProviderProposal } from "../api/createProposalApi";
import type { CreateProviderProposalBody } from "../api/createProposalTypes";

export function useCreateProviderProposal(requestId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateProviderProposalBody) => createProviderProposal(requestId, body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.provider.proposals() });
      void queryClient.invalidateQueries({ queryKey: queryKeys.provider.requests() });
    },
  });
}
