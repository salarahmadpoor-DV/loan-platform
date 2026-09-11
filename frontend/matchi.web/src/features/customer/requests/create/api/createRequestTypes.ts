import type { RequestKind } from "../../../../../shared/types/marketplace";

export type CreateRequestServiceLine = {
  serviceId: number;
  quantity: number;
  description?: string | null;
};

export type CreateRequestProductLine = {
  productId?: number | null;
  productCategoryId?: number | null;
  quantity: number;
  unit?: string | null;
  description?: string | null;
};

export type CreateRequestBody = {
  requestType: RequestKind;
  title: string;
  description?: string | null;
  services?: CreateRequestServiceLine[];
  products?: CreateRequestProductLine[];
};

export type CreateRequestResponse = {
  requestId: number;
};

export type CatalogService = {
  id: number;
  name: string;
  categoryId: number;
};

export type CatalogServiceList = {
  page: number;
  pageSize: number;
  items: CatalogService[];
};
