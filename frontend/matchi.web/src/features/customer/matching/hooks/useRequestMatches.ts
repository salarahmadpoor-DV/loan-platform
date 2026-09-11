import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getRequestMatches } from "../api/matchingApi";

export function useRequestMatches(requestId: number | undefined) {
  return useQuery({
    queryKey:
      requestId != null
        ? queryKeys.matching.byRequest(requestId)
        : queryKeys.matching.all,
    queryFn: () => getRequestMatches(requestId as number),
    enabled: requestId != null && Number.isFinite(requestId) && requestId > 0,
  });
}
