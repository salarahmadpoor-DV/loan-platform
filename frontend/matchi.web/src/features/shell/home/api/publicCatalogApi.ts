import { getJson } from "../../../../shared/api/httpClient";
import type { CatalogServiceList } from "../../../customer/requests/create/api/createRequestTypes";

export type ServiceCategoryDto = {
  id: number;
  name: string;
};

export function getServiceCategories(): Promise<ServiceCategoryDto[]> {
  return getJson<ServiceCategoryDto[]>("/api/services/categories");
}

export function getPublicCatalogServices(): Promise<CatalogServiceList> {
  return getJson<CatalogServiceList>("/api/services", {
    params: { page: 1, pageSize: 40 },
  });
}
