import { getJson } from "../../../../../shared/api/httpClient";
import type { ProviderCatalogProduct, ProviderCatalogService } from "./providerCatalogTypes";

export function getMyProviderServices(): Promise<ProviderCatalogService[]> {
  return getJson<ProviderCatalogService[]>("/api/providers/me/services");
}

export function getMyProviderProducts(): Promise<ProviderCatalogProduct[]> {
  return getJson<ProviderCatalogProduct[]>("/api/providers/me/products");
}
