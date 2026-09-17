import {
  canAccessWorkspace,
  defaultWorkspacePath,
} from "../auth/workspaces";

export const customerCreateRequestPath = "/customer/requests/create";
export const providerWorkspacePath = "/provider/dashboard";

export function createRequestPathWithQuery(query?: string): string {
  const q = query?.trim();
  if (!q) {
    return customerCreateRequestPath;
  }
  return `${customerCreateRequestPath}?q=${encodeURIComponent(q)}`;
}

export function loginPathWithNext(next: string): string {
  return `/login?next=${encodeURIComponent(next)}`;
}

/** Only same-origin app paths. Blocks protocol-relative and absolute URLs. */
export function safeInternalPath(raw: string | null | undefined): string | null {
  if (!raw) {
    return null;
  }
  if (!raw.startsWith("/") || raw.startsWith("//") || raw.includes("://")) {
    return null;
  }
  return raw;
}

export function findServicePath(
  isAuthenticated: boolean,
  roles: readonly string[] | undefined,
  query?: string,
): string {
  const destination = createRequestPathWithQuery(query);
  if (canAccessWorkspace("customer", roles)) {
    return destination;
  }
  if (isAuthenticated) {
    return defaultWorkspacePath(roles);
  }
  return loginPathWithNext(destination);
}

export function professionalJoinPath(
  isAuthenticated: boolean,
  roles: readonly string[] | undefined,
): string {
  if (canAccessWorkspace("provider", roles)) {
    return providerWorkspacePath;
  }
  if (isAuthenticated) {
    return "/#for-professionals";
  }
  return loginPathWithNext(providerWorkspacePath);
}
