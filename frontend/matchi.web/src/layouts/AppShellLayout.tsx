import {
  AppBar,
  Box,
  Button,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Stack,
  Toolbar,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { t } from "../shared/i18n";
import { useAuth } from "../shared/auth/AuthProvider";
import { resolveWorkspaces } from "../shared/auth/workspaces";
import {
  workspaceHome,
  workspaceLabelKey,
  workspaceNav,
  type AppWorkspace,
} from "../shared/navigation/navModel";

const DRAWER_WIDTH = 260;

type AppShellLayoutProps = {
  workspace: AppWorkspace;
};

export function AppShellLayout({ workspace }: AppShellLayoutProps) {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();
  const { logout, user } = useAuth();
  const availableWorkspaces = resolveWorkspaces(user?.roles);
  const items = workspaceNav[workspace];
  const drawerAnchor = theme.direction === "rtl" ? "right" : "left";

  const drawer = (
    <Box sx={{ pt: 1 }} onClick={() => setMobileOpen(false)}>
      <Typography variant="subtitle2" color="text.secondary" sx={{ px: 2, py: 1 }}>
        {t(workspaceLabelKey[workspace])}
      </Typography>
      <List>
        {items.map((item) => (
          <ListItemButton
            key={item.to}
            component={NavLink}
            to={item.to}
            end={item.to === workspaceHome[workspace]}
            sx={{
              "&.active": {
                bgcolor: "action.selected",
              },
            }}
          >
            <ListItemText primary={t(item.labelKey)} />
          </ListItemButton>
        ))}
      </List>
    </Box>
  );

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: theme.direction === "rtl" ? "row-reverse" : "row",
        minHeight: "100vh",
      }}
    >
      <AppBar
        position="fixed"
        elevation={0}
        sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}
      >
        <Toolbar sx={{ gap: 1, px: { xs: 1, sm: 2 } }}>
          {!isDesktop ? (
            <IconButton
              color="inherit"
              aria-label={t("nav.openMenu")}
              onClick={() => setMobileOpen(true)}
            >
              <Typography component="span" fontWeight={700}>
                ≡
              </Typography>
            </IconButton>
          ) : null}
          <Typography variant="h6" sx={{ flexGrow: { xs: 1, md: 0 }, mr: { md: 2 } }}>
            {t("app.name")}
          </Typography>
          {isDesktop ? (
            <Stack direction="row" spacing={0.5} sx={{ flexGrow: 1 }}>
              {availableWorkspaces.map((ws) => (
                <Button
                  key={ws}
                  color="inherit"
                  size="small"
                  variant={ws === workspace ? "outlined" : "text"}
                  onClick={() => navigate(workspaceHome[ws])}
                >
                  {t(workspaceLabelKey[ws])}
                </Button>
              ))}
            </Stack>
          ) : null}
          <Button
            color="inherit"
            onClick={() => {
              logout();
            }}
          >
            {t("auth.signOut")}
          </Button>
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={{ width: { md: DRAWER_WIDTH }, flexShrink: { md: 0 } }}
      >
        {isDesktop ? (
          <Drawer
            variant="permanent"
            open
            anchor={drawerAnchor}
            sx={{
              "& .MuiDrawer-paper": {
                width: DRAWER_WIDTH,
                boxSizing: "border-box",
                top: 56,
                height: "calc(100% - 56px)",
              },
            }}
          >
            {drawer}
          </Drawer>
        ) : (
          <Drawer
            variant="temporary"
            open={mobileOpen}
            anchor={drawerAnchor}
            onClose={() => setMobileOpen(false)}
            ModalProps={{ keepMounted: true }}
            sx={{
              "& .MuiDrawer-paper": { width: DRAWER_WIDTH, boxSizing: "border-box" },
            }}
          >
            {drawer}
          </Drawer>
        )}
      </Box>
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: { xs: 2, sm: 3 },
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          mt: "56px",
        }}
      >
        {!isDesktop ? (
          <Stack direction="row" spacing={1} sx={{ mb: 2, flexWrap: "wrap" }}>
            {availableWorkspaces.map((ws) => (
              <Button
                key={ws}
                size="small"
                variant={ws === workspace ? "contained" : "outlined"}
                onClick={() => navigate(workspaceHome[ws])}
              >
                {t(workspaceLabelKey[ws])}
              </Button>
            ))}
          </Stack>
        ) : null}
        <Outlet />
      </Box>
    </Box>
  );
}
