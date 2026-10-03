import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../shared/api/queryKeys";
import { useAuth } from "../../../shared/auth/AuthProvider";
import { workspaceHome, type AppWorkspace } from "../../../shared/navigation/navModel";
import { getMyWorkspaceState, setLastWorkspace, setPreferredWorkspace } from "../api/workspaceApi";
import { isAppWorkspace } from "../model/workspacePath";

function asWorkspaceList(values: readonly string[] | undefined): AppWorkspace[] {
  return (values ?? []).filter(isAppWorkspace);
}

export function useWorkspaceStateQuery() {
  const { isAuthenticated, user } = useAuth();
  const userId = user?.id ?? 0;
  return useQuery({
    queryKey: queryKeys.workspace.state(userId),
    queryFn: getMyWorkspaceState,
    enabled: isAuthenticated,
  });
}

export function useWorkspaceMutations() {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const key = queryKeys.workspace.state(user?.id ?? 0);

  const persistLast = useMutation({
    mutationFn: setLastWorkspace,
    onSuccess: (state) => {
      queryClient.setQueryData(key, state);
    },
  });

  const persistPreferred = useMutation({
    mutationFn: setPreferredWorkspace,
    onSuccess: (state) => {
      queryClient.setQueryData(key, state);
    },
  });

  return { persistLast, persistPreferred };
}

export function resolvedWorkspacePath(resolved: string | null | undefined): string {
  if (!isAppWorkspace(resolved)) {
    return "/";
  }
  return workspaceHome[resolved];
}

export function parseWorkspaceState(data: ReturnType<typeof useWorkspaceStateQuery>["data"]) {
  const preferred = data?.preferredWorkspace;
  const last = data?.lastWorkspace;
  const resolved = data?.resolvedWorkspace;
  return {
    workspaces: asWorkspaceList(data?.availableWorkspaces),
    preferredWorkspace: isAppWorkspace(preferred) ? preferred : null,
    lastWorkspace: isAppWorkspace(last) ? last : null,
    resolvedWorkspace: isAppWorkspace(resolved) ? resolved : null,
  };
}
