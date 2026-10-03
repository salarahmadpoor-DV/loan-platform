import {
  parseWorkspaceState,
  resolvedWorkspacePath,
  useWorkspaceStateQuery,
} from "../../features/workspace/hooks/useWorkspaceState";
import { useAuth } from "./AuthProvider";
import type { WorkspaceCapabilities } from "./workspaces";
import type { AppWorkspace } from "../navigation/navModel";

export function useWorkspaceAccess() {
  const { isAuthenticated } = useAuth();
  const query = useWorkspaceStateQuery();
  const parsed = parseWorkspaceState(query.data);
  const capabilities: WorkspaceCapabilities | undefined =
    isAuthenticated && query.isFetched
      ? {
          hasProviderProfile: parsed.workspaces.includes("provider"),
          ownsBusiness: parsed.workspaces.includes("business"),
        }
      : undefined;

  const isReady = !isAuthenticated || query.isFetched;
  const defaultPath = resolvedWorkspacePath(parsed.resolvedWorkspace);

  return {
    isReady,
    capabilities,
    workspaces: parsed.workspaces,
    preferredWorkspace: parsed.preferredWorkspace,
    lastWorkspace: parsed.lastWorkspace,
    resolvedWorkspace: parsed.resolvedWorkspace,
    defaultPath,
    canAccess: (workspace: AppWorkspace) => parsed.workspaces.includes(workspace),
    isError: query.isError,
    error: query.error,
  };
}
