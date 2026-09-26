import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import {
  addMyProviderProduct,
  addMyProviderService,
  deleteMyProviderProduct,
  deleteMyProviderService,
  updateMyProviderProduct,
  updateMyProviderService,
} from "../../proposals/create/api/providerCatalogApi";
import { useMyProviderProducts, useMyProviderServices } from "../../proposals/create/hooks/useProviderCatalog";

export { useMyProviderProducts, useMyProviderServices };

export function useProviderOfferingMutations() {
  const queryClient = useQueryClient();
  const onSuccess = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: queryKeys.provider.myServices() }),
      queryClient.invalidateQueries({ queryKey: queryKeys.provider.myProducts() }),
    ]);

  return {
    addService: useMutation({
      mutationFn: addMyProviderService,
      onSuccess,
    }),
    updateService: useMutation({
      mutationFn: ({ serviceId, isActive }: { serviceId: number; isActive: boolean }) =>
        updateMyProviderService(serviceId, isActive),
      onSuccess,
    }),
    deleteService: useMutation({
      mutationFn: deleteMyProviderService,
      onSuccess,
    }),
    addProduct: useMutation({
      mutationFn: addMyProviderProduct,
      onSuccess,
    }),
    updateProduct: useMutation({
      mutationFn: ({
        productId,
        ...body
      }: {
        productId: number;
        price?: number | null;
        isAvailable: boolean;
        minOrderQuantity?: number | null;
        leadTimeDays?: number | null;
      }) => updateMyProviderProduct(productId, body),
      onSuccess,
    }),
    deleteProduct: useMutation({
      mutationFn: deleteMyProviderProduct,
      onSuccess,
    }),
  };
}
