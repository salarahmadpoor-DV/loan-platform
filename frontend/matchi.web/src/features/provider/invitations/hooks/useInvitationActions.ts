import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { QueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { acceptProviderInvitation, rejectProviderInvitation } from "../api/invitationsApi";

async function refreshMembershipQueries(queryClient: QueryClient) {
  await Promise.all([
    queryClient.invalidateQueries({ queryKey: queryKeys.provider.invitations() }),
    queryClient.invalidateQueries({ queryKey: queryKeys.provider.memberships() }),
    queryClient.invalidateQueries({ queryKey: [...queryKeys.provider.all, "owned"] }),
  ]);
}

export function useAcceptProviderInvitation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (businessProviderId: number) => acceptProviderInvitation(businessProviderId),
    onSuccess: async () => {
      await refreshMembershipQueries(queryClient);
    },
  });
}

export function useRejectProviderInvitation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (businessProviderId: number) => rejectProviderInvitation(businessProviderId),
    onSuccess: async () => {
      await refreshMembershipQueries(queryClient);
    },
  });
}
