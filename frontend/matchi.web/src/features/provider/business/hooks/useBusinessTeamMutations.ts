import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import {
  inviteBusinessProvider,
  removeBusinessProvider,
  type InviteProviderBody,
} from "../api/ownedBusinessApi";

export function useInviteBusinessProvider(businessId: number | null) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: InviteProviderBody) => {
      if (businessId == null) {
        return Promise.reject(new Error("businessId required"));
      }
      return inviteBusinessProvider(businessId, body);
    },
    onSuccess: async () => {
      if (businessId != null) {
        await queryClient.invalidateQueries({ queryKey: queryKeys.provider.ownedTeam(businessId) });
      }
    },
  });
}

export function useRemoveBusinessProvider(businessId: number | null) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (providerId: number) => removeBusinessProvider(providerId, businessId ?? undefined),
    onSuccess: async () => {
      if (businessId != null) {
        await queryClient.invalidateQueries({ queryKey: queryKeys.provider.ownedTeam(businessId) });
      }
    },
  });
}
