import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getOwnedBusinessTeam } from "../api/ownedBusinessApi";

export function useOwnedBusinessTeam(businessId: number | null) {
  return useQuery({
    queryKey: queryKeys.provider.ownedTeam(businessId ?? 0),
    queryFn: () => getOwnedBusinessTeam(businessId ?? undefined),
    enabled: businessId != null && businessId > 0,
  });
}
