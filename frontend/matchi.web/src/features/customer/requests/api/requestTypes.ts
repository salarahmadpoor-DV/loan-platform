import { t } from "../../../../shared/i18n";
import { REQUEST_KINDS, type RequestKind } from "../../../../shared/types/marketplace";

export type RequestServiceAttribute = {
  serviceAttributeId: number;
  value: string | null;
};

export type RequestProductAttribute = {
  productAttributeId: number;
  value: string | null;
};

export type RequestServiceLine = {
  id: number;
  serviceId: number;
  quantity: number;
  description: string | null;
  displayOrder: number;
  attributes: RequestServiceAttribute[];
};

export type RequestProductLine = {
  id: number;
  productId: number | null;
  productCategoryId: number | null;
  quantity: number;
  unit: string | null;
  description: string | null;
  displayOrder: number;
  attributes: RequestProductAttribute[];
};

export type RequestLocation = {
  province: string | null;
  city: string | null;
  district: string | null;
  address: string | null;
  lat: number | null;
  lng: number | null;
};

export type RequestSchedule = {
  date: string;
  timeFrom: string | null;
  timeTo: string | null;
  isFlexible: boolean;
};

/** Customer-owned Request DTO from GET /api/requests/me and GET /api/requests/{id}. */
export type RequestDto = {
  id: number;
  customerId: number;
  requestType: string;
  title: string;
  description: string | null;
  status: string;
  services: RequestServiceLine[];
  products: RequestProductLine[];
  location: RequestLocation | null;
  schedule: RequestSchedule | null;
  createDate: string;
  updateDate: string | null;
};

export function isRequestKind(value: string): value is RequestKind {
  return (REQUEST_KINDS as readonly string[]).includes(value);
}

export function requestKindLabel(requestType: string): string {
  if (requestType === "Service") {
    return t("request.kind.Service");
  }
  if (requestType === "Product") {
    return t("request.kind.Product");
  }
  if (requestType === "Hybrid") {
    return t("request.kind.Hybrid");
  }
  return requestType;
}
