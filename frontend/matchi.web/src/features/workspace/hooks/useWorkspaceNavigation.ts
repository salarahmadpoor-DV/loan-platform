import { useNavigate } from "react-router-dom";
import { workspaceHome, type AppWorkspace } from "../../../shared/navigation/navModel";
import { useWorkspaceAccess } from "../../../shared/auth/useWorkspaceAccess";
import { workspaceFromPath } from "../model/workspacePath";
import { useWorkspaceMutations } from "./useWorkspaceState";

export function useWorkspaceNavigation() {
  const navigate = useNavigate();
  const access = useWorkspaceAccess();
  const { persistLast, persistPreferred } = useWorkspaceMutations();

  async function switchTo(workspace: AppWorkspace) {
    if (!access.canAccess(workspace)) {
      return false;
    }
    await persistLast.mutateAsync(workspace);
    navigate(workspaceHome[workspace]);
    return true;
  }

  async function setPreferred(workspace: AppWorkspace) {
    if (!access.canAccess(workspace)) {
      return false;
    }
    await persistPreferred.mutateAsync(workspace);
    navigate(workspaceHome[workspace]);
    return true;
  }

  async function openPath(path: string, currentWorkspace?: AppWorkspace) {
    const required = workspaceFromPath(path);
    if (!required || required === currentWorkspace) {
      navigate(path);
      return true;
    }
    if (!access.canAccess(required)) {
      return false;
    }
    await persistLast.mutateAsync(required);
    navigate(path);
    return true;
  }

  return {
    switchTo,
    setPreferred,
    openPath,
    persistLast,
    persistPreferred,
    preferredWorkspace: access.preferredWorkspace,
    workspaces: access.workspaces,
    canAccess: access.canAccess,
  };
}
