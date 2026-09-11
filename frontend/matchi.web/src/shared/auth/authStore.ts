import { create } from "zustand";
import { decodeAccessToken, isTokenExpired } from "./jwt";
import { readAccessToken, writeAccessToken } from "./tokenStorage";

export type AuthUser = {
  id: number;
  mobile: string;
  roles: string[];
};

type AuthState = {
  accessToken: string | null;
  user: AuthUser | null;
  setSession: (accessToken: string, user: AuthUser) => void;
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
  setSession: (accessToken, user) => {
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
