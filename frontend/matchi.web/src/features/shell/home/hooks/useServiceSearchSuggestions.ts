import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getPublicCatalogServices } from "../api/publicCatalogApi";

export function useServiceSearchSuggestions() {
  return useQuery({
    queryKey: [...queryKeys.catalog.services, "public-home"] as const,
    queryFn: getPublicCatalogServices,
  });
}
