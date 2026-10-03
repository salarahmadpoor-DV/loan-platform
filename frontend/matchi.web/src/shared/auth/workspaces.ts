import { workspaceHome, type AppWorkspace } from "../navigation/navModel";
import { normalizeRoleCodes } from "./jwt";

export type WorkspaceCapabilities = {
  hasProviderProfile: boolean;
  ownsBusiness: boolean;
};

/**
 * Client helpers for JWT ADMIN/USER plus capability flags.
 * Startup workspace resolution is authoritative on the backend
 * (`GET /api/users/me/workspace`). These helpers remain for public-path
 * routing that still receives capability flags.
 */
export function resolveWorkspaces(
  roles: readonly string[] | undefined,
  capabilities?: WorkspaceCapabilities,
): AppWorkspace[] {
  const set = new Set(normalizeRoleCodes(roles));

  if (set.has("ADMIN")) {
    return ["customer", "provider", "business"];
  }

  const workspaces: AppWorkspace[] = [];
  if (set.has("USER")) {
    workspaces.push("customer");
  }
  if (capabilities?.hasProviderProfile) {
    workspaces.push("provider");
  }
  if (capabilities?.ownsBusiness) {
    workspaces.push("business");
  }
  return workspaces;
}

export function defaultWorkspacePath(
  roles: readonly string[] | undefined,
  capabilities?: WorkspaceCapabilities,
): string {
  const available = resolveWorkspaces(roles, capabilities);
  if (available.length === 0) {
    return "/";
  }
  if (available.includes("provider")) {
    return workspaceHome.provider;
  }
  return workspaceHome[available[0]];
}

export function canAccessWorkspace(
  workspace: AppWorkspace,
  roles: readonly string[] | undefined,
  capabilities?: WorkspaceCapabilities,
): boolean {
  return resolveWorkspaces(roles, capabilities).includes(workspace);
}
