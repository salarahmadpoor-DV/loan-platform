import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../../shared/api/queryKeys";
import { getCatalogServices } from "../api/createRequestApi";

export function useCatalogServices() {
  return useQuery({
    queryKey: [...queryKeys.catalog.services, "list"] as const,
    queryFn: getCatalogServices,
  });
}
