import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderProfile } from "../api/providerProfileApi";

export function useMyProviderProfile() {
  return useQuery({
    queryKey: queryKeys.provider.profile(),
    queryFn: getMyProviderProfile,
  });
}
