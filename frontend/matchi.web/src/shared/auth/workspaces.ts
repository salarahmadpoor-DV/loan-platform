import {
  workspaceHome,
  type AppWorkspace,
} from "../navigation/navModel";

/**
 * Live Matchi JWTs typically include USER (and sometimes ADMIN).
 * PROVIDER and BUSINESS_OWNER are accepted when present.
 * Permission claims (PROVIDER_*) are not used — every USER may have those.
 */
export function resolveWorkspaces(roles: readonly string[] | undefined): AppWorkspace[] {
  const set = new Set((roles ?? []).map((role) => role.toUpperCase()));

  if (set.has("ADMIN")) {
    return ["customer", "provider", "business"];
  }

  const workspaces: AppWorkspace[] = ["customer"];
  if (set.has("PROVIDER")) {
    workspaces.push("provider");
  }
  if (set.has("BUSINESS_OWNER")) {
    workspaces.push("business");
  }
  return workspaces;
}

export function defaultWorkspacePath(roles: readonly string[] | undefined): string {
  const first = resolveWorkspaces(roles)[0] ?? "customer";
  return workspaceHome[first];
}

export function canAccessWorkspace(
  workspace: AppWorkspace,
  roles: readonly string[] | undefined,
): boolean {
  return resolveWorkspaces(roles).includes(workspace);
}
