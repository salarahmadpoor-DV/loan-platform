import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderDeals } from "../api/providerDealsApi";

export function useMyProviderDeals() {
  return useQuery({
    queryKey: queryKeys.provider.deals(),
    queryFn: getMyProviderDeals,
  });
}
