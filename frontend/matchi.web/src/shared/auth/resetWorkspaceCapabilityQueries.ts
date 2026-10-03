import type { QueryClient } from "@tanstack/react-query";
import { queryKeys } from "../api/queryKeys";

/** Drops user-scoped caches so login/logout cannot reuse another user's data. */
export function resetWorkspaceCapabilityQueries(queryClient: QueryClient): void {
  queryClient.removeQueries({ queryKey: queryKeys.provider.profile() });
  queryClient.removeQueries({ queryKey: queryKeys.provider.myBusinesses() });
  queryClient.removeQueries({ queryKey: queryKeys.notifications.all });
  queryClient.removeQueries({ queryKey: queryKeys.workspace.all });
}
