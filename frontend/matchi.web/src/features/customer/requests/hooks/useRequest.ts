import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getRequestById } from "../api/requestsApi";

export function useRequest(requestId: number | undefined) {
  return useQuery({
    queryKey:
      requestId != null ? queryKeys.requests.detail(requestId) : queryKeys.requests.all,
    queryFn: () => getRequestById(requestId as number),
    enabled: requestId != null && Number.isFinite(requestId) && requestId > 0,
  });
}
