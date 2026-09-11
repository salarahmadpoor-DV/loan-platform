import { getJson, postJson } from "../../../../../shared/api/httpClient";
import type {
  CatalogServiceList,
  CreateRequestBody,
  CreateRequestResponse,
} from "./createRequestTypes";

export function createRequest(body: CreateRequestBody): Promise<CreateRequestResponse> {
  return postJson<CreateRequestResponse, CreateRequestBody>("/api/requests", body);
}

/** Existing public catalog list. Used to populate the service picker. */
export function getCatalogServices(): Promise<CatalogServiceList> {
  return getJson<CatalogServiceList>("/api/services", {
    params: { page: 1, pageSize: 100 },
  });
}
