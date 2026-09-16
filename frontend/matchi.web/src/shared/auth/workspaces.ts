import { workspaceHome, type AppWorkspace } from "../navigation/navModel";
import { normalizeRoleCodes } from "./jwt";

/**
 * Workspace access from JWT **role codes** only (not `permission` claims).
 *
 * USER → Customer
 * PROVIDER → Provider (Customer only if USER or ADMIN is also present)
 * BUSINESS_OWNER → Business
 * ADMIN → Customer + Provider + Business
 *
 * `PROVIDER_VIEW` / other permission strings are ignored.
 */
export function resolveWorkspaces(roles: readonly string[] | undefined): AppWorkspace[] {
  const set = new Set(normalizeRoleCodes(roles));

  if (set.has("ADMIN")) {
    return ["customer", "provider", "business"];
  }

  const workspaces: AppWorkspace[] = [];
  if (set.has("USER")) {
    workspaces.push("customer");
  }
  if (set.has("PROVIDER")) {
    workspaces.push("provider");
  }
  if (set.has("BUSINESS_OWNER")) {
    workspaces.push("business");
  }
  return workspaces;
}

export function defaultWorkspacePath(roles: readonly string[] | undefined): string {
  const available = resolveWorkspaces(roles);
  if (available.length === 0) {
    return "/";
  }
  return workspaceHome[available[0]];
}

export function canAccessWorkspace(
  workspace: AppWorkspace,
  roles: readonly string[] | undefined,
): boolean {
  return resolveWorkspaces(roles).includes(workspace);
}
