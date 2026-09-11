import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getDealById } from "../api/dealsApi";

export function useDeal(dealId: number | undefined) {
  return useQuery({
    queryKey: dealId != null ? queryKeys.deals.detail(dealId) : queryKeys.deals.all,
    queryFn: () => getDealById(dealId as number),
    enabled: dealId != null && Number.isFinite(dealId) && dealId > 0,
  });
}
