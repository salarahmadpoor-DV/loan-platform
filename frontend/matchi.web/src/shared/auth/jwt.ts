export type AccessTokenClaims = {
  userId: number;
  mobile: string;
  roles: string[];
  exp: number | null;
};

/** .NET `ClaimTypes.Role` as written into Matchi JWTs by `JwtTokenService`. */
export const ROLE_CLAIM_URI =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

const NAME_ID_URI =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const MOBILE_URI =
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
  if (typeof value === "string") {
    const trimmed = value.trim();
    if (!trimmed) {
      return [];
    }
    if (trimmed.startsWith("[")) {
      try {
        return asStringArray(JSON.parse(trimmed) as unknown);
      } catch {
        return [trimmed];
      }
    }
    return [trimmed];
  }
  if (Array.isArray(value)) {
    return value.flatMap((item) => asStringArray(item));
  }
  return [];
}

function isRoleClaimKey(key: string): boolean {
  const lower = key.toLowerCase();
  if (lower === "permission" || lower === "permissions" || lower.endsWith("/claims/permission")) {
    return false;
  }
  return (
    lower === "role" ||
    lower === "roles" ||
    key === ROLE_CLAIM_URI ||
    lower.endsWith("/identity/claims/role") ||
    lower.endsWith("/claims/role")
  );
}

/** Collect role codes from a JWT payload. Does not use permission claims. */
export function extractRolesFromPayload(payload: Record<string, unknown>): string[] {
  const collected: string[] = [];
  for (const [key, value] of Object.entries(payload)) {
    if (isRoleClaimKey(key)) {
      collected.push(...asStringArray(value));
    }
  }
  return normalizeRoleCodes(collected);
}

export function normalizeRoleCodes(roles: readonly string[] | undefined): string[] {
  const seen = new Set<string>();
  const result: string[] = [];
  for (const raw of roles ?? []) {
    const code = raw.trim().toUpperCase();
    if (!code || seen.has(code)) {
      continue;
    }
    seen.add(code);
    result.push(code);
  }
  return result;
}

export function decodeAccessToken(token: string): AccessTokenClaims | null {
  const payload = parseJwtPayload(token);
  if (!payload) {
    return null;
  }

  const sub = payload.sub ?? payload[NAME_ID_URI] ?? payload.nameid;
  const userId =
    typeof sub === "string" ? Number.parseInt(sub, 10) : typeof sub === "number" ? sub : Number.NaN;
  if (!Number.isFinite(userId)) {
    return null;
  }

  const mobileRaw = payload[MOBILE_URI] ?? payload.mobile ?? payload.phone_number;
  const mobile = typeof mobileRaw === "string" ? mobileRaw : "";
  const roles = extractRolesFromPayload(payload);
  const exp = typeof payload.exp === "number" ? payload.exp : null;

  return { userId, mobile, roles, exp };
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
