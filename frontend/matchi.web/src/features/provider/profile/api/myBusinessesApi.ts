import { getJson } from "../../../../shared/api/httpClient";
import type { MyBusinessProfile } from "./myBusinessTypes";

export function getMyBusinesses(): Promise<MyBusinessProfile[]> {
  return getJson<MyBusinessProfile[]>("/api/businesses/me");
}
