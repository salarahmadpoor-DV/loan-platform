import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderProposals } from "../api/providerProposalsApi";

export function useMyProviderProposals() {
  return useQuery({
    queryKey: queryKeys.provider.proposals(),
    queryFn: getMyProviderProposals,
  });
}
