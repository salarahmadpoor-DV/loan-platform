import { getJson, putJson } from "../../../shared/api/httpClient";
import type { AppWorkspace } from "../../../shared/navigation/navModel";
import type { WorkspaceState } from "./workspaceTypes";

export function getMyWorkspaceState(): Promise<WorkspaceState> {
  return getJson<WorkspaceState>("/api/users/me/workspace");
}

export function setPreferredWorkspace(workspace: AppWorkspace): Promise<WorkspaceState> {
  return putJson<WorkspaceState>("/api/users/me/workspace-preference", { workspace });
}

export function setLastWorkspace(workspace: AppWorkspace): Promise<WorkspaceState> {
  return putJson<WorkspaceState>("/api/users/me/workspace-last", { workspace });
}
