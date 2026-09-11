import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderExecutions } from "../api/providerExecutionsApi";

export function useMyProviderExecutions() {
  return useQuery({
    queryKey: queryKeys.provider.executions(),
    queryFn: getMyProviderExecutions,
  });
}
