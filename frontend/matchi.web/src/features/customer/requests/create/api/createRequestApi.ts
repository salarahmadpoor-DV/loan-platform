import { getJson, postJson } from "../../../../../shared/api/httpClient";
import type {
  CatalogAttribute,
  CatalogCategory,
  CatalogProductList,
  CatalogServiceDetail,
  CatalogServiceList,
  CreateRequestBody,
  CreateRequestResponse,
} from "./createRequestTypes";

export function createRequest(body: CreateRequestBody): Promise<CreateRequestResponse> {
  return postJson<CreateRequestResponse, CreateRequestBody>("/api/requests", body);
}

export function getServiceCategories(): Promise<CatalogCategory[]> {
  return getJson<CatalogCategory[]>("/api/services/categories");
}

export function getCatalogServices(categoryId?: number): Promise<CatalogServiceList> {
  return getJson<CatalogServiceList>("/api/services", {
    params: {
      page: 1,
      pageSize: 100,
      ...(categoryId ? { categoryId } : {}),
    },
  });
}

export function getCatalogServiceDetail(serviceId: number): Promise<CatalogServiceDetail> {
  return getJson<CatalogServiceDetail>(`/api/services/${serviceId}`);
}

export function getProductCategories(): Promise<CatalogCategory[]> {
  return getJson<CatalogCategory[]>("/api/products/categories");
}

export function getCatalogProducts(categoryId?: number): Promise<CatalogProductList> {
  return getJson<CatalogProductList>("/api/products", {
    params: {
      page: 1,
      pageSize: 100,
      ...(categoryId ? { categoryId } : {}),
    },
  });
}

export function getProductCategoryAttributes(categoryId: number): Promise<CatalogAttribute[]> {
  return getJson<CatalogAttribute[]>(`/api/products/categories/${categoryId}/attributes`);
}
