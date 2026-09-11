import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../../shared/api/queryKeys";
import { getMyProviderProducts, getMyProviderServices } from "../api/providerCatalogApi";

export function useMyProviderServices() {
  return useQuery({
    queryKey: queryKeys.provider.myServices(),
    queryFn: getMyProviderServices,
  });
}

export function useMyProviderProducts() {
  return useQuery({
    queryKey: queryKeys.provider.myProducts(),
    queryFn: getMyProviderProducts,
  });
}
