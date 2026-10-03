import type { AppWorkspace } from "../../../shared/navigation/navModel";

const KNOWN: readonly AppWorkspace[] = ["customer", "provider", "business"];

export function isAppWorkspace(value: string | null | undefined): value is AppWorkspace {
  return value === "customer" || value === "provider" || value === "business";
}

export function workspaceFromPath(path: string): AppWorkspace | null {
  for (const workspace of KNOWN) {
    if (path === `/${workspace}` || path.startsWith(`/${workspace}/`)) {
      return workspace;
    }
  }
  return null;
}
