import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderMemberships } from "../api/providerMembershipsApi";

export function useMyProviderMemberships() {
  return useQuery({
    queryKey: queryKeys.provider.memberships(),
    queryFn: getMyProviderMemberships,
  });
}
