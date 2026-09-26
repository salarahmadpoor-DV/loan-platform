import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { updateOwnedBusiness, type UpdateOwnedBusinessBody } from "../api/ownedBusinessApi";

export function useUpdateOwnedBusiness() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateOwnedBusinessBody) => updateOwnedBusiness(body),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.myBusinesses() });
    },
  });
}
