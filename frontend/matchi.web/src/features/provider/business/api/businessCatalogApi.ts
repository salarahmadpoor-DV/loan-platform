import { deleteJson, getJson, postJson, putJson } from "../../../../shared/api/httpClient";
import type { BusinessCatalogProduct, BusinessCatalogService } from "./businessCatalogTypes";

function withBusiness(businessId: number) {
  return { params: { businessId } };
}

export function getOwnedBusinessServices(businessId: number): Promise<BusinessCatalogService[]> {
  return getJson("/api/businesses/me/services", withBusiness(businessId));
}

export function getOwnedBusinessProducts(businessId: number): Promise<BusinessCatalogProduct[]> {
  return getJson("/api/businesses/me/products", withBusiness(businessId));
}

export function addOwnedBusinessService(
  businessId: number,
  body: {
    serviceId: number;
    isActive?: boolean;
    canCustomerChooseProvider?: boolean;
    minPrice?: number | null;
    maxPrice?: number | null;
  },
): Promise<{ id: number; serviceId: number }> {
  return postJson("/api/businesses/me/services", { businessId, ...body });
}

export function updateOwnedBusinessService(
  businessId: number,
  serviceId: number,
  body: {
    isActive: boolean;
    canCustomerChooseProvider: boolean;
    minPrice?: number | null;
    maxPrice?: number | null;
  },
): Promise<{ serviceId: number; success: boolean }> {
  return putJson(`/api/businesses/me/services/${serviceId}`, { businessId, ...body });
}

export function deleteOwnedBusinessService(businessId: number, serviceId: number): Promise<void> {
  return deleteJson(`/api/businesses/me/services/${serviceId}`, withBusiness(businessId));
}

export function addOwnedBusinessProduct(
  businessId: number,
  body: {
    productId: number;
    price?: number | null;
    isAvailable?: boolean;
  },
): Promise<{ id: number; productId: number }> {
  return postJson("/api/businesses/me/products", { businessId, ...body });
}

export function updateOwnedBusinessProduct(
  businessId: number,
  productId: number,
  body: {
    price?: number | null;
    isAvailable: boolean;
    minOrderQuantity?: number | null;
    leadTimeDays?: number | null;
  },
): Promise<{ productId: number; success: boolean }> {
  return putJson(`/api/businesses/me/products/${productId}`, { businessId, ...body });
}

export function deleteOwnedBusinessProduct(businessId: number, productId: number): Promise<void> {
  return deleteJson(`/api/businesses/me/products/${productId}`, withBusiness(businessId));
}
