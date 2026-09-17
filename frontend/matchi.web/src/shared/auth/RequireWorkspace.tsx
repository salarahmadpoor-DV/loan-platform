import { Navigate, Outlet } from "react-router-dom";
import { LoadingState } from "../ui/LoadingState";
import type { AppWorkspace } from "../navigation/navModel";
import { useWorkspaceAccess } from "./useWorkspaceAccess";

type RequireWorkspaceProps = {
  workspace: AppWorkspace;
};

export function RequireWorkspace({ workspace }: RequireWorkspaceProps) {
  const access = useWorkspaceAccess();
  if (!access.isReady) {
    return <LoadingState />;
  }
  if (access.canAccess(workspace)) {
    return <Outlet />;
  }
  if (access.workspaces.length === 0 || access.defaultPath === "/") {
    return <Navigate to="/" replace />;
  }
  return <Navigate to={access.defaultPath} replace />;
}
