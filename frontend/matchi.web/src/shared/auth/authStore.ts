import { create } from "zustand";
import { decodeAccessToken, isTokenExpired, normalizeRoleCodes } from "./jwt";
import { readAccessToken, writeAccessToken } from "./tokenStorage";

export type AuthUser = {
  id: number;
  mobile: string;
  roles: string[];
};

type AuthState = {
  accessToken: string | null;
  user: AuthUser | null;
  setSession: (accessToken: string, profile?: Partial<AuthUser>) => void;
  clearSession: () => void;
};

function userFromAccessToken(token: string): AuthUser | null {
  const claims = decodeAccessToken(token);
  if (!claims || isTokenExpired(claims)) {
    return null;
  }
  return {
    id: claims.userId,
    mobile: claims.mobile,
    roles: claims.roles,
  };
}

function mergeSessionUser(token: string, profile?: Partial<AuthUser>): AuthUser | null {
  const fromToken = userFromAccessToken(token);
  if (!fromToken) {
    return null;
  }
  return {
    id: profile?.id && Number.isFinite(profile.id) ? profile.id : fromToken.id,
    mobile: (profile?.mobile && profile.mobile.trim()) || fromToken.mobile,
    roles: normalizeRoleCodes([...(fromToken.roles ?? []), ...(profile?.roles ?? [])]),
  };
}

function hydrateSession(): Pick<AuthState, "accessToken" | "user"> {
  const token = readAccessToken();
  if (!token) {
    return { accessToken: null, user: null };
  }
  const user = userFromAccessToken(token);
  if (!user) {
    writeAccessToken(null);
    return { accessToken: null, user: null };
  }
  return { accessToken: token, user };
}

export const useAuthStore = create<AuthState>((set) => ({
  ...hydrateSession(),
  setSession: (accessToken, profile) => {
    const user = mergeSessionUser(accessToken, profile);
    if (!user) {
      writeAccessToken(null);
      set({ accessToken: null, user: null });
      return;
    }
    writeAccessToken(accessToken);
    set({ accessToken, user });
  },
  clearSession: () => {
    writeAccessToken(null);
    set({ accessToken: null, user: null });
  },
}));

export function isAuthenticated(): boolean {
  return Boolean(useAuthStore.getState().accessToken);
}
