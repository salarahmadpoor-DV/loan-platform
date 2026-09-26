import { deleteJson, getJson, postJson, putJson } from "../../../../../shared/api/httpClient";
import type { ProviderCatalogProduct, ProviderCatalogService } from "./providerCatalogTypes";

export function getMyProviderServices(): Promise<ProviderCatalogService[]> {
  return getJson<ProviderCatalogService[]>("/api/providers/me/services");
}

export function getMyProviderProducts(): Promise<ProviderCatalogProduct[]> {
  return getJson<ProviderCatalogProduct[]>("/api/providers/me/products");
}

export function addMyProviderService(serviceId: number): Promise<{ id: number; serviceId: number }> {
  return postJson("/api/providers/me/services", { serviceId });
}

export function updateMyProviderService(
  serviceId: number,
  isActive: boolean,
): Promise<{ serviceId: number; success: boolean }> {
  return putJson(`/api/providers/me/services/${serviceId}`, { isActive });
}

export function deleteMyProviderService(serviceId: number): Promise<void> {
  return deleteJson(`/api/providers/me/services/${serviceId}`);
}

export function addMyProviderProduct(body: {
  productId: number;
  price?: number | null;
  isAvailable?: boolean;
}): Promise<{ id: number; productId: number }> {
  return postJson("/api/providers/me/products", body);
}

export function updateMyProviderProduct(
  productId: number,
  body: {
    price?: number | null;
    isAvailable: boolean;
    minOrderQuantity?: number | null;
    leadTimeDays?: number | null;
  },
): Promise<{ productId: number; success: boolean }> {
  return putJson(`/api/providers/me/products/${productId}`, body);
}

export function deleteMyProviderProduct(productId: number): Promise<void> {
  return deleteJson(`/api/providers/me/products/${productId}`);
}
