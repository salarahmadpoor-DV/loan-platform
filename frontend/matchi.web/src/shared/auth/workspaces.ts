import { workspaceHome, type AppWorkspace } from "../navigation/navModel";
import { normalizeRoleCodes } from "./jwt";

/**
 * Workspace access from JWT **role codes** only (not `permission` claims).
 *
 * USER → Customer
 * PROVIDER → Provider (also Customer if USER or ADMIN is present)
 * BUSINESS_OWNER → Business
 * ADMIN → Customer + Provider + Business
 *
 * Default landing: PROVIDER role opens `/provider/dashboard` even when USER is also present.
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
  const codes = new Set(normalizeRoleCodes(roles));
  const available = resolveWorkspaces(roles);
  if (available.length === 0) {
    return "/";
  }
  // USER is always granted on OTP, so USER+PROVIDER must not land in Customer.
  if (codes.has("PROVIDER") && available.includes("provider")) {
    return workspaceHome.provider;
  }
  return workspaceHome[available[0]];
}

export function canAccessWorkspace(
  workspace: AppWorkspace,
  roles: readonly string[] | undefined,
): boolean {
  return resolveWorkspaces(roles).includes(workspace);
}
