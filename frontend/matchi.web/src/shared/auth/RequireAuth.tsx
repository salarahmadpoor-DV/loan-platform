import { Navigate, Outlet, useLocation } from "react-router-dom";
import { loginPathWithNext } from "../marketplace/publicPaths";
import { useAuth } from "./AuthProvider";

export function RequireAuth() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();
  if (!isAuthenticated) {
    return (
      <Navigate
        to={loginPathWithNext(`${location.pathname}${location.search}`)}
        replace
      />
    );
  }
  return <Outlet />;
}
