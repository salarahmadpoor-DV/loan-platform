import { useQueryClient } from "@tanstack/react-query";
import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  type ReactNode,
} from "react";
import { useNavigate } from "react-router-dom";
import { setUnauthorizedHandler } from "../api/httpClient";
import { useAuthStore, type AuthUser } from "./authStore";
import { resetWorkspaceCapabilityQueries } from "./resetWorkspaceCapabilityQueries";

type AuthContextValue = {
  accessToken: string | null;
  user: AuthUser | null;
  isAuthenticated: boolean;
  setSession: (accessToken: string, profile?: Partial<AuthUser>) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const accessToken = useAuthStore((s) => s.accessToken);
  const user = useAuthStore((s) => s.user);
  const setSessionStore = useAuthStore((s) => s.setSession);
  const clearSession = useAuthStore((s) => s.clearSession);

  useEffect(() => {
    setUnauthorizedHandler(() => {
      resetWorkspaceCapabilityQueries(queryClient);
      if (window.location.pathname !== "/login") {
        navigate("/login", { replace: true });
      }
    });
    return () => setUnauthorizedHandler(null);
  }, [navigate, queryClient]);

  const value = useMemo<AuthContextValue>(
    () => ({
      accessToken,
      user,
      isAuthenticated: Boolean(accessToken),
      setSession: (token, profile) => {
        resetWorkspaceCapabilityQueries(queryClient);
        setSessionStore(token, profile);
      },
      logout: () => {
        resetWorkspaceCapabilityQueries(queryClient);
        clearSession();
        navigate("/login", { replace: true });
      },
    }),
    [accessToken, user, setSessionStore, clearSession, navigate, queryClient],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within AuthProvider.");
  }
  return ctx;
}
