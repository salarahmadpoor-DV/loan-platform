import { useEffect, useState } from "react";
import {
  Box,
  Collapse,
  List,
  ListItemButton,
  ListItemText,
  Typography,
} from "@mui/material";
import { NavLink } from "react-router-dom";
import { t } from "../shared/i18n";
import type { AppWorkspace, NavGroup, NavItem } from "../shared/navigation/navModel";
import {
  groupContainsPath,
  navItemActive,
  workspaceHome,
  workspaceLabelKey,
} from "../shared/navigation/navModel";
import { CheckOutlined, ExpandLess, ExpandMore, NavIcon } from "../shared/ui/icons";
import { rtlSafeFlexRow } from "../shared/ui/noflipFlex";
import { matchiColors } from "../app/designTokens";

type WorkspaceDrawerNavProps = {
  workspace: AppWorkspace;
  home: NavItem;
  groups: readonly NavGroup[];
  pathname: string;
  direction: "ltr" | "rtl";
  availableWorkspaces: readonly AppWorkspace[];
  showSessionTools: boolean;
  localeLabel: string;
  onToggleLocale: () => void;
  onLogout: () => void;
  onSwitchWorkspace: (workspace: AppWorkspace) => void;
  onSetPreferredWorkspace?: (workspace: AppWorkspace) => void;
  preferredWorkspace?: AppWorkspace | null;
  onNavigate?: () => void;
};

function activeIndicator(rtl: boolean): string {
  return rtl ? `inset -3px 0 0 ${matchiColors.primary}` : `inset 3px 0 0 ${matchiColors.primary}`;
}

export function WorkspaceDrawerNav({
  workspace,
  home,
  groups,
  pathname,
  direction,
  availableWorkspaces,
  showSessionTools,
  localeLabel,
  onToggleLocale,
  onLogout,
  onSwitchWorkspace,
  onSetPreferredWorkspace,
  preferredWorkspace,
  onNavigate,
}: WorkspaceDrawerNavProps) {
  const rtl = direction === "rtl";
  const homePath = workspaceHome[workspace];
  const routeGroup = groups.find((group) => groupContainsPath(group, pathname, homePath));
  const [openGroup, setOpenGroup] = useState<string | null>(routeGroup?.id ?? null);

  useEffect(() => {
    setOpenGroup(routeGroup?.id ?? null);
  }, [routeGroup?.id]);

  const rowStyle = {
    ...rtlSafeFlexRow,
    width: "100%" as const,
    gap: 8,
  };

  function toggleGroup(id: string) {
    setOpenGroup((current) => (current === id ? null : id));
  }

  function linkSx(active: boolean) {
    return {
      mx: 1,
      borderRadius: 1,
      minHeight: 44,
      py: 0.75,
      bgcolor: active ? "action.selected" : "transparent",
      color: active ? "primary.main" : "text.primary",
      "&:hover": { bgcolor: "action.hover" },
    };
  }

  const sessionGroupOpen = openGroup === "session";

  return (
    <Box dir={direction} sx={{ pb: 2 }}>
      <Box
        sx={{
          px: 2,
          py: 1.5,
          borderBottom: 1,
          borderColor: "divider",
        }}
      >
        <Typography variant="subtitle1" fontWeight={700}>
          {t("app.name")}
        </Typography>
        <Typography variant="caption" color="text.secondary" display="block">
          {t("nav.drawer.tagline")}
        </Typography>
        <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 0.25 }}>
          {t(workspaceLabelKey[workspace])}
        </Typography>
      </Box>

      <List disablePadding sx={{ pt: 1 }}>
        <ListItemButton
          component={NavLink}
          to={home.to}
          end
          onClick={onNavigate}
          style={{ ...rowStyle, boxShadow: navItemActive(home, pathname, homePath) ? activeIndicator(rtl) : undefined }}
          sx={linkSx(navItemActive(home, pathname, homePath))}
        >
          <NavIcon name={home.icon ?? "dashboard"} aria-hidden sx={{ color: "inherit", flexShrink: 0 }} />
          <ListItemText
            primary={t(home.labelKey)}
            primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
            sx={{ my: 0, flex: 1, minWidth: 0 }}
          />
        </ListItemButton>

        {groups.map((group) => {
          const open = openGroup === group.id;
          return (
            <Box key={group.id} sx={{ mt: 0.5 }}>
              <ListItemButton
                onClick={() => toggleGroup(group.id)}
                aria-expanded={open}
                style={rowStyle}
                sx={{
                  mx: 1,
                  borderRadius: 1,
                  minHeight: 40,
                  py: 0.5,
                  color: "text.secondary",
                  "&:hover": { bgcolor: "action.hover" },
                }}
              >
                <NavIcon name={group.icon} aria-hidden sx={{ fontSize: 20, color: "primary.main", flexShrink: 0 }} />
                <ListItemText
                  primary={t(group.labelKey)}
                  primaryTypographyProps={{ variant: "caption", fontWeight: 600, letterSpacing: "0.02em" }}
                  sx={{ my: 0, flex: 1, minWidth: 0 }}
                />
                {open ? (
                  <ExpandLess aria-hidden sx={{ fontSize: 20, color: "text.secondary" }} />
                ) : (
                  <ExpandMore aria-hidden sx={{ fontSize: 20, color: "text.secondary" }} />
                )}
              </ListItemButton>
              <Collapse in={open} timeout="auto" unmountOnExit>
                <List disablePadding>
                  {group.items.map((item) => {
                    const active = navItemActive(item, pathname, homePath);
                    return (
                      <ListItemButton
                        key={item.to}
                        component={NavLink}
                        to={item.to}
                        end={item.end ?? item.to === homePath}
                        onClick={onNavigate}
                        style={{
                          ...rowStyle,
                          paddingInlineStart: 36,
                          paddingInlineEnd: 12,
                          boxShadow: active ? activeIndicator(rtl) : undefined,
                        }}
                        sx={linkSx(active)}
                      >
                        <ListItemText
                          primary={t(item.labelKey)}
                          primaryTypographyProps={{ variant: "body2", fontWeight: active ? 600 : 500 }}
                          sx={{ my: 0, flex: 1, minWidth: 0 }}
                        />
                      </ListItemButton>
                    );
                  })}
                </List>
              </Collapse>
            </Box>
          );
        })}

        {showSessionTools ? (
          <Box sx={{ mt: 0.5 }}>
            <ListItemButton
              onClick={() => toggleGroup("session")}
              aria-expanded={sessionGroupOpen}
              style={rowStyle}
              sx={{
                mx: 1,
                borderRadius: 1,
                minHeight: 40,
                py: 0.5,
                color: "text.secondary",
                "&:hover": { bgcolor: "action.hover" },
              }}
            >
              <NavIcon name="settings" aria-hidden sx={{ fontSize: 20, color: "primary.main", flexShrink: 0 }} />
              <ListItemText
                primary={t("nav.section.settings")}
                primaryTypographyProps={{ variant: "caption", fontWeight: 600, letterSpacing: "0.02em" }}
                sx={{ my: 0, flex: 1, minWidth: 0 }}
              />
              {sessionGroupOpen ? (
                <ExpandLess aria-hidden sx={{ fontSize: 20 }} />
              ) : (
                <ExpandMore aria-hidden sx={{ fontSize: 20 }} />
              )}
            </ListItemButton>
            <Collapse in={sessionGroupOpen} timeout="auto" unmountOnExit>
              <List disablePadding>
                <Typography variant="caption" color="text.secondary" sx={{ display: "block", px: 2.5, pt: 0.5 }}>
                  {t("nav.switchWorkspace")}
                </Typography>
                {availableWorkspaces.map((ws) => (
                  <ListItemButton
                    key={ws}
                    selected={ws === workspace}
                    onClick={() => {
                      onSwitchWorkspace(ws);
                      onNavigate?.();
                    }}
                    style={{
                      ...rowStyle,
                      paddingInlineStart: 36,
                      paddingInlineEnd: 12,
                    }}
                    sx={linkSx(ws === workspace)}
                  >
                    {ws === workspace ? (
                      <CheckOutlined sx={{ fontSize: 18, color: "primary.main", flexShrink: 0 }} aria-hidden />
                    ) : null}
                    <ListItemText
                      primary={t(workspaceLabelKey[ws])}
                      secondary={preferredWorkspace === ws ? t("workspace.default") : undefined}
                      primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
                      secondaryTypographyProps={{ variant: "caption" }}
                      sx={{ my: 0, flex: 1, minWidth: 0 }}
                    />
                  </ListItemButton>
                ))}
                {onSetPreferredWorkspace && preferredWorkspace !== workspace ? (
                  <ListItemButton
                    onClick={() => {
                      onSetPreferredWorkspace(workspace);
                      onNavigate?.();
                    }}
                    style={{
                      ...rowStyle,
                      paddingInlineStart: 36,
                      paddingInlineEnd: 12,
                    }}
                    sx={linkSx(false)}
                  >
                    <ListItemText
                      primary={t("workspace.setDefault")}
                      primaryTypographyProps={{ variant: "body2" }}
                      sx={{ my: 0, flex: 1 }}
                    />
                  </ListItemButton>
                ) : null}
                <ListItemButton
                  onClick={() => {
                    onToggleLocale();
                  }}
                  style={{ ...rowStyle, paddingInlineStart: 36, paddingInlineEnd: 12 }}
                  sx={linkSx(false)}
                >
                  <ListItemText primary={localeLabel} primaryTypographyProps={{ variant: "body2" }} sx={{ my: 0, flex: 1 }} />
                </ListItemButton>
                <ListItemButton
                  onClick={() => {
                    onLogout();
                    onNavigate?.();
                  }}
                  style={{ ...rowStyle, paddingInlineStart: 36, paddingInlineEnd: 12 }}
                  sx={linkSx(false)}
                >
                  <ListItemText primary={t("auth.signOut")} primaryTypographyProps={{ variant: "body2" }} sx={{ my: 0, flex: 1 }} />
                </ListItemButton>
              </List>
            </Collapse>
          </Box>
        ) : null}
      </List>
    </Box>
  );
}
