export type BusinessCatalogService = {
  serviceId: number;
  serviceName: string | null;
  isActive: boolean;
  canCustomerChooseProvider: boolean;
  minPrice: number | null;
  maxPrice: number | null;
};

export type BusinessCatalogProduct = {
  productId: number;
  productName: string | null;
  price: number | null;
  isAvailable: boolean;
  minOrderQuantity: number | null;
  leadTimeDays: number | null;
};
