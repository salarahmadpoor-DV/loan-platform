import type { RequestKind } from "../../../../../shared/types/marketplace";

export type CreateRequestServiceLine = {
  serviceId: number;
  quantity: number;
  description?: string | null;
  attributes?: Array<{ serviceAttributeId: number; value?: string | null }>;
};

export type CreateRequestProductLine = {
  productId?: number | null;
  productCategoryId?: number | null;
  quantity: number;
  unit?: string | null;
  description?: string | null;
  attributes?: Array<{ productAttributeId: number; value?: string | null }>;
};

export type CreateRequestLocation = {
  provinceId: number;
  cityId: number;
  districtId: number;
  address?: string | null;
  lat?: number | null;
  lng?: number | null;
};

export type CreateRequestBody = {
  requestType: RequestKind;
  title: string;
  description?: string | null;
  services?: CreateRequestServiceLine[];
  products?: CreateRequestProductLine[];
  location?: CreateRequestLocation | null;
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

export type CatalogCategory = {
  id: number;
  name: string;
  parentId?: number | null;
  displayOrder?: number;
};

export type CatalogProduct = {
  id: number;
  name: string;
  categoryId: number;
};

export type CatalogProductList = {
  page: number;
  pageSize: number;
  items: CatalogProduct[];
};

export type CatalogAttributeOption = {
  id: number;
  value: string;
  displayName: string;
  displayOrder: number;
};

export type CatalogAttribute = {
  id: number;
  name: string;
  code: string;
  dataType: string;
  isRequired: boolean;
  displayOrder: number;
  options: CatalogAttributeOption[];
};

export type CatalogServiceDetail = {
  id: number;
  name: string;
  categoryId: number;
  attributeCount: number;
  attributes: CatalogAttribute[];
};
