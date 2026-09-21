import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { useAuthStore } from "../../../../shared/auth/authStore";
import { createMyProvider, type CreateProviderBody } from "../api/providerProfileApi";

export function useCreateMyProvider() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateProviderBody) => createMyProvider(body),
    onSuccess: async () => {
      const userId = useAuthStore.getState().user?.id ?? 0;
      queryClient.setQueryData([...queryKeys.provider.profile(), "exists", userId], true);
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.profile() });
    },
  });
}
