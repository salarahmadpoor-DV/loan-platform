import { useQueries } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getProposalById } from "../api/proposalsApi";

export function useProposalDetails(ids: number[]) {
  return useQueries({
    queries: ids.map((id) => ({
      queryKey: queryKeys.proposals.detail(id),
      queryFn: () => getProposalById(id),
      enabled: id > 0,
    })),
  });
}
