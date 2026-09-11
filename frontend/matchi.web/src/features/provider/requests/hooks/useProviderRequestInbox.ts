import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getProviderRequestInbox } from "../api/providerRequestsApi";

export function useProviderRequestInbox() {
  return useQuery({
    queryKey: queryKeys.provider.requests(),
    queryFn: getProviderRequestInbox,
  });
}
