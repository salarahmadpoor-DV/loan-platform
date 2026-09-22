import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../../shared/api/queryKeys";
import {
  getCatalogProducts,
  getCatalogServiceDetail,
  getCatalogServices,
  getProductCategories,
  getProductCategoryAttributes,
  getServiceCategories,
} from "../api/createRequestApi";

export function useServiceCategories() {
  return useQuery({
    queryKey: queryKeys.catalog.serviceCategories,
    queryFn: getServiceCategories,
  });
}

export function useCatalogServices(categoryId?: number) {
  return useQuery({
    queryKey: [...queryKeys.catalog.services, categoryId ?? "all"] as const,
    queryFn: () => getCatalogServices(categoryId),
    enabled: categoryId == null || categoryId > 0,
  });
}

export function useCatalogServiceDetail(serviceId?: number) {
  return useQuery({
    queryKey: queryKeys.catalog.serviceDetail(serviceId ?? 0),
    queryFn: () => getCatalogServiceDetail(serviceId as number),
    enabled: serviceId != null && serviceId > 0,
  });
}

export function useProductCategories() {
  return useQuery({
    queryKey: queryKeys.catalog.productCategories,
    queryFn: getProductCategories,
  });
}

export function useCatalogProducts(categoryId?: number) {
  return useQuery({
    queryKey: queryKeys.catalog.products(categoryId),
    queryFn: () => getCatalogProducts(categoryId),
    enabled: categoryId != null && categoryId > 0,
  });
}

export function useProductCategoryAttributes(categoryId?: number) {
  return useQuery({
    queryKey: queryKeys.catalog.productAttributes(categoryId ?? 0),
    queryFn: () => getProductCategoryAttributes(categoryId as number),
    enabled: categoryId != null && categoryId > 0,
  });
}
