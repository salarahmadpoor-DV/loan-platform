import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyBusinesses } from "../api/myBusinessesApi";

export function useMyBusinesses() {
  return useQuery({
    queryKey: queryKeys.provider.myBusinesses(),
    queryFn: getMyBusinesses,
  });
}
