import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderServiceAreas } from "../api/providerProfileApi";

export function useMyProviderServiceAreas() {
  return useQuery({
    queryKey: queryKeys.provider.myAreas(),
    queryFn: getMyProviderServiceAreas,
  });
}
