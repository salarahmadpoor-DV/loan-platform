import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { useAuthStore } from "../../../../shared/auth/authStore";
import { createOwnedBusiness, type CreateOwnedBusinessBody } from "../api/ownedBusinessApi";

export function useCreateOwnedBusiness() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateOwnedBusinessBody) => createOwnedBusiness(body),
    onSuccess: async () => {
      const userId = useAuthStore.getState().user?.id ?? 0;
      queryClient.setQueryData([...queryKeys.provider.myBusinesses(), "owned", userId], true);
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.myBusinesses() });
    },
  });
}
