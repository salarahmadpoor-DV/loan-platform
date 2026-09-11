import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyDeals } from "../api/dealsApi";

export function useMyDeals() {
  return useQuery({
    queryKey: queryKeys.deals.mine(),
    queryFn: getMyDeals,
  });
}
