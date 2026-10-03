/**
 * Allows only same-origin in-app paths for SPA navigation.
 * Rejects absolute URLs, protocol-relative URLs, backslashes, and non-http(s) schemes.
 */
export function resolveInAppNotificationPath(
  actionUrl: string | null | undefined,
): string | null {
  if (!actionUrl) {
    return null;
  }

  const trimmed = actionUrl.trim();
  if (!trimmed.startsWith("/") || trimmed.startsWith("//") || trimmed.includes("\\")) {
    return null;
  }
  if (/[\u0000-\u001F\u007F]/.test(trimmed)) {
    return null;
  }

  try {
    const origin = window.location.origin;
    const parsed = new URL(trimmed, origin);
    if (parsed.origin !== origin) {
      return null;
    }
    if (parsed.protocol !== "http:" && parsed.protocol !== "https:") {
      return null;
    }
    return `${parsed.pathname}${parsed.search}${parsed.hash}`;
  } catch {
    return null;
  }
}

export function isInAppNotificationPath(actionUrl: string | null | undefined): actionUrl is string {
  return resolveInAppNotificationPath(actionUrl) !== null;
}
