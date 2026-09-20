import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../api/queryKeys";
import { ApiError } from "../api/errors";
import { getMyBusinesses } from "../../features/provider/profile/api/myBusinessesApi";
import { getMyProviderProfile } from "../../features/provider/profile/api/providerProfileApi";
import { useAuth } from "./AuthProvider";
import { normalizeRoleCodes } from "./jwt";
import {
  canAccessWorkspace,
  defaultWorkspacePath,
  resolveWorkspaces,
  type WorkspaceCapabilities,
} from "./workspaces";
import type { AppWorkspace } from "../navigation/navModel";

function isAdmin(roles: readonly string[] | undefined): boolean {
  return normalizeRoleCodes(roles).includes("ADMIN");
}

async function providerProfileExists(): Promise<boolean> {
  try {
    await getMyProviderProfile();
    return true;
  } catch (error) {
    // 404: authenticated but no Providers.UserId row (legacy PROVIDER_VIEW path).
    // 403: ProviderWorkspace policy failed (no Provider row, or unauthenticated mapping).
    if (error instanceof ApiError && (error.status === 404 || error.status === 403)) {
      return false;
    }
    throw error;
  }
}

async function userOwnsBusiness(): Promise<boolean> {
  try {
    const owned = await getMyBusinesses();
    return owned.length > 0;
  } catch (error) {
    if (error instanceof ApiError && (error.status === 404 || error.status === 403)) {
      return false;
    }
    throw error;
  }
}

export function useWorkspaceAccess() {
  const { isAuthenticated, user } = useAuth();
  const admin = isAdmin(user?.roles);
  const enabled = isAuthenticated && !admin;

  const userId = user?.id;

  const provider = useQuery({
    queryKey: [...queryKeys.provider.profile(), "exists", userId ?? 0],
    queryFn: providerProfileExists,
    enabled,
    retry: false,
  });

  const businesses = useQuery({
    queryKey: [...queryKeys.provider.myBusinesses(), "owned", userId ?? 0],
    queryFn: userOwnsBusiness,
    enabled,
    retry: false,
  });

  const capabilities: WorkspaceCapabilities | undefined = admin
    ? { hasProviderProfile: true, ownsBusiness: true }
    : enabled && provider.isFetched && businesses.isFetched
      ? {
          hasProviderProfile: provider.data === true,
          ownsBusiness: businesses.data === true,
        }
      : undefined;

  const isReady = !isAuthenticated || admin || (provider.isFetched && businesses.isFetched);

  const workspaces = resolveWorkspaces(user?.roles, capabilities);
  const defaultPath = defaultWorkspacePath(user?.roles, capabilities);

  return {
    isReady,
    capabilities,
    workspaces,
    defaultPath,
    canAccess: (workspace: AppWorkspace) =>
      canAccessWorkspace(workspace, user?.roles, capabilities),
  };
}
