import { Navigate, Outlet } from "react-router-dom";
import type { AppWorkspace } from "../navigation/navModel";
import { useAuth } from "./AuthProvider";
import { canAccessWorkspace, defaultWorkspacePath, resolveWorkspaces } from "./workspaces";

type RequireWorkspaceProps = {
  workspace: AppWorkspace;
};

export function RequireWorkspace({ workspace }: RequireWorkspaceProps) {
  const { user } = useAuth();
  if (canAccessWorkspace(workspace, user?.roles)) {
    return <Outlet />;
  }
  const fallback = defaultWorkspacePath(user?.roles);
  const available = resolveWorkspaces(user?.roles);
  if (available.length === 0 || fallback === "/") {
    return <Navigate to="/" replace />;
  }
  return <Navigate to={fallback} replace />;
}
