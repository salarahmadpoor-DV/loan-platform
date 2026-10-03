export type WorkspaceState = {
  availableWorkspaces: string[];
  preferredWorkspace: string | null;
  lastWorkspace: string | null;
  resolvedWorkspace: string | null;
};
