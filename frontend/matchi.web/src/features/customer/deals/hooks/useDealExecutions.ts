import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getDealExecutions } from "../api/dealsApi";

export function useDealExecutions(dealId: number | undefined) {
  return useQuery({
    queryKey:
      dealId != null ? queryKeys.executions.byDeal(dealId) : queryKeys.executions.all,
    queryFn: () => getDealExecutions(dealId as number),
    enabled: dealId != null && Number.isFinite(dealId) && dealId > 0,
  });
}
