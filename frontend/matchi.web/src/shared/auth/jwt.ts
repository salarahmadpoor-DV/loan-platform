export type AccessTokenClaims = {
  userId: number;
  mobile: string;
  roles: string[];
  exp: number | null;
};

const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const NAME_ID =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const MOBILE =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone";

function parseJwtPayload(token: string): Record<string, unknown> | null {
  const parts = token.split(".");
  if (parts.length < 2) {
    return null;
  }

  try {
    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64 + "=".repeat((4 - (base64.length % 4)) % 4);
    const json = decodeURIComponent(
      atob(padded)
        .split("")
        .map((c) => `%${c.charCodeAt(0).toString(16).padStart(2, "0")}`)
        .join(""),
    );
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

function asStringArray(value: unknown): string[] {
  if (typeof value === "string" && value.length > 0) {
    return [value];
  }
  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === "string" && item.length > 0);
  }
  return [];
}

export function decodeAccessToken(token: string): AccessTokenClaims | null {
  const payload = parseJwtPayload(token);
  if (!payload) {
    return null;
  }

  const sub = payload.sub ?? payload[NAME_ID];
  const userId =
    typeof sub === "string" ? Number.parseInt(sub, 10) : typeof sub === "number" ? sub : Number.NaN;
  if (!Number.isFinite(userId)) {
    return null;
  }

  const mobileRaw = payload[MOBILE] ?? payload.mobile;
  const mobile = typeof mobileRaw === "string" ? mobileRaw : "";
  const roles = [
    ...asStringArray(payload.role),
    ...asStringArray(payload[ROLE_CLAIM]),
    ...asStringArray(payload.roles),
  ];
  const uniqueRoles = [...new Set(roles)];
  const exp = typeof payload.exp === "number" ? payload.exp : null;

  return { userId, mobile, roles: uniqueRoles, exp };
}

export function isTokenExpired(
  claims: AccessTokenClaims,
  nowSeconds = Date.now() / 1000,
): boolean {
  if (claims.exp == null) {
    return false;
  }
  return claims.exp <= nowSeconds;
}
