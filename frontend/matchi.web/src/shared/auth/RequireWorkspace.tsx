import { Navigate, Outlet } from "react-router-dom";
import type { AppWorkspace } from "../navigation/navModel";
import { useAuth } from "./AuthProvider";
import { canAccessWorkspace, defaultWorkspacePath } from "./workspaces";

type RequireWorkspaceProps = {
  workspace: AppWorkspace;
};

export function RequireWorkspace({ workspace }: RequireWorkspaceProps) {
  const { user } = useAuth();
  if (!canAccessWorkspace(workspace, user?.roles)) {
    return <Navigate to={defaultWorkspacePath(user?.roles)} replace />;
  }
  return <Outlet />;
}
