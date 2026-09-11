import { getJson } from "../../../../shared/api/httpClient";
import type { RequestDto } from "./requestTypes";

/**
 * Live list contract is GET /api/requests/me (owner-scoped).
 * GET /api/requests is not a customer list endpoint.
 */
export function getMyRequests(): Promise<RequestDto[]> {
  return getJson<RequestDto[]>("/api/requests/me");
}

export function getRequestById(requestId: number): Promise<RequestDto> {
  return getJson<RequestDto>(`/api/requests/${requestId}`);
}
