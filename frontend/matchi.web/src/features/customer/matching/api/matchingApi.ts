import { getJson } from "../../../../shared/api/httpClient";
import type { MatchResult } from "./matchingTypes";

export function getRequestMatches(requestId: number): Promise<MatchResult[]> {
  return getJson<MatchResult[]>(`/api/requests/${requestId}/matches`);
}
