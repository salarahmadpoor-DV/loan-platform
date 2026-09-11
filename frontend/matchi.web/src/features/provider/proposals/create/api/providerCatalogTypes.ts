/** Live GET /api/providers/me/services (`ProviderServiceDto`). */
export type ProviderCatalogService = {
  serviceId: number;
  serviceName: string | null;
  isActive: boolean;
};

/** Live GET /api/providers/me/products (`ProviderProductDto`). */
export type ProviderCatalogProduct = {
  productId: number;
  productName: string | null;
  price: number | null;
  isAvailable: boolean;
  minOrderQuantity: number | null;
  leadTimeDays: number | null;
};
