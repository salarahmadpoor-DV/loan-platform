import type { QueryClient } from "@tanstack/react-query";
import { queryKeys } from "../api/queryKeys";

/** Drops provider/business capability caches so login cannot reuse a stale `exists: false`. */
export function resetWorkspaceCapabilityQueries(queryClient: QueryClient): void {
  queryClient.removeQueries({ queryKey: queryKeys.provider.profile() });
  queryClient.removeQueries({ queryKey: queryKeys.provider.myBusinesses() });
}
