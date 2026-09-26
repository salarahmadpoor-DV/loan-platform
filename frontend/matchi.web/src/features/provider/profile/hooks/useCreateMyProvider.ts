import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { useAuthStore } from "../../../../shared/auth/authStore";
import { persistOnboardServiceArea } from "../api/persistOnboardServiceArea";
import { createMyProvider, type CreateProviderBody } from "../api/providerProfileApi";
import type { ServiceAreaSelection } from "../model/serviceArea";

export type CreateMyProviderInput = {
  body: CreateProviderBody;
  serviceArea: ServiceAreaSelection | null;
};

export function useCreateMyProvider() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ body, serviceArea }: CreateMyProviderInput) => {
      const result = await createMyProvider(body);
      try {
        await persistOnboardServiceArea(serviceArea);
      } catch {
        return { ...result, areaSaved: false as const };
      }
      return { ...result, areaSaved: true as const };
    },
    onSuccess: async () => {
      const userId = useAuthStore.getState().user?.id ?? 0;
      queryClient.setQueryData([...queryKeys.provider.profile(), "exists", userId], true);
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.profile() });
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.myAreas() });
    },
  });
}
