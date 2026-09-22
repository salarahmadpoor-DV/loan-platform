import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import {
  addOwnedBusinessProduct,
  addOwnedBusinessService,
  deleteOwnedBusinessProduct,
  deleteOwnedBusinessService,
  getOwnedBusinessProducts,
  getOwnedBusinessServices,
  updateOwnedBusinessProduct,
  updateOwnedBusinessService,
} from "../api/businessCatalogApi";

export function useOwnedBusinessServices(businessId: number | null) {
  return useQuery({
    queryKey: queryKeys.provider.ownedServices(businessId ?? 0),
    queryFn: () => getOwnedBusinessServices(businessId as number),
    enabled: businessId != null && businessId > 0,
  });
}

export function useOwnedBusinessProducts(businessId: number | null) {
  return useQuery({
    queryKey: queryKeys.provider.ownedProducts(businessId ?? 0),
    queryFn: () => getOwnedBusinessProducts(businessId as number),
    enabled: businessId != null && businessId > 0,
  });
}

export function useOwnedBusinessCatalogMutations(businessId: number) {
  const queryClient = useQueryClient();
  const onSuccess = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: queryKeys.provider.ownedServices(businessId) }),
      queryClient.invalidateQueries({ queryKey: queryKeys.provider.ownedProducts(businessId) }),
    ]);

  return {
    addService: useMutation({
      mutationFn: (body: Parameters<typeof addOwnedBusinessService>[1]) =>
        addOwnedBusinessService(businessId, body),
      onSuccess,
    }),
    updateService: useMutation({
      mutationFn: (input: {
        serviceId: number;
        isActive: boolean;
        canCustomerChooseProvider: boolean;
        minPrice?: number | null;
        maxPrice?: number | null;
      }) => updateOwnedBusinessService(businessId, input.serviceId, input),
      onSuccess,
    }),
    deleteService: useMutation({
      mutationFn: (serviceId: number) => deleteOwnedBusinessService(businessId, serviceId),
      onSuccess,
    }),
    addProduct: useMutation({
      mutationFn: (body: Parameters<typeof addOwnedBusinessProduct>[1]) =>
        addOwnedBusinessProduct(businessId, body),
      onSuccess,
    }),
    updateProduct: useMutation({
      mutationFn: (input: {
        productId: number;
        price?: number | null;
        isAvailable: boolean;
        minOrderQuantity?: number | null;
        leadTimeDays?: number | null;
      }) => updateOwnedBusinessProduct(businessId, input.productId, input),
      onSuccess,
    }),
    deleteProduct: useMutation({
      mutationFn: (productId: number) => deleteOwnedBusinessProduct(businessId, productId),
      onSuccess,
    }),
  };
}
