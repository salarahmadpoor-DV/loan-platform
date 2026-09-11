import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getRequestProposals } from "../api/proposalsApi";

export function useRequestProposals(requestId: number | undefined) {
  return useQuery({
    queryKey:
      requestId != null
        ? queryKeys.proposals.byRequest(requestId)
        : queryKeys.proposals.all,
    queryFn: () => getRequestProposals(requestId as number),
    enabled: requestId != null && Number.isFinite(requestId) && requestId > 0,
  });
}
