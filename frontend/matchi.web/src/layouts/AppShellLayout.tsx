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
import { PageContainer } from "../shared/ui/PageContainer";

const DRAWER_WIDTH = 260;
const APP_BAR_HEIGHT = 56;

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
    <Box sx={{ pt: 1, px: 0.5 }} onClick={() => setMobileOpen(false)}>
      <Typography variant="overline" color="text.secondary" sx={{ px: 2, py: 1, display: "block" }}>
        {t(workspaceLabelKey[workspace])}
      </Typography>
      <List disablePadding>
        {items.map((item) => (
          <ListItemButton
            key={item.to}
            component={NavLink}
            to={item.to}
            end={item.to === workspaceHome[workspace]}
            sx={{
              mx: 1,
              borderRadius: 1,
              minHeight: 48,
              "&.active": {
                bgcolor: "action.selected",
                color: "primary.main",
                fontWeight: 700,
                borderRight: theme.direction === "rtl" ? 0 : 3,
                borderLeft: theme.direction === "rtl" ? 3 : 0,
                borderColor: "primary.main",
              },
            }}
          >
          <ListItemText
              primary={t(item.labelKey)}
              primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
            />
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
        minWidth: 0,
      }}
    >
      <AppBar
        position="fixed"
        elevation={0}
        color="inherit"
        sx={{
          zIndex: (t) => t.zIndex.drawer + 1,
          borderBottom: 1,
          borderColor: "divider",
          bgcolor: "background.paper",
          color: "text.primary",
        }}
      >
        <Toolbar
          sx={{
            gap: 1,
            px: { xs: 1, sm: 2 },
            minHeight: APP_BAR_HEIGHT,
            flexWrap: { xs: "wrap", sm: "nowrap" },
          }}
        >
          {!isDesktop ? (
            <IconButton
              color="inherit"
              aria-label={t("nav.openMenu")}
              onClick={() => setMobileOpen(true)}
            >
              <Typography component="span" fontWeight={700} aria-hidden>
                ≡
              </Typography>
            </IconButton>
          ) : null}
          <Typography
            variant="subtitle1"
            component="p"
            sx={{ flexGrow: { xs: 1, md: 0 }, minWidth: 0 }}
            noWrap
          >
            {t("app.name")}
          </Typography>
          {isDesktop ? (
            <Stack
              direction="row"
              spacing={0.5}
              sx={{ flexGrow: 1, minWidth: 0, flexWrap: "wrap", rowGap: 0.5 }}
            >
              {availableWorkspaces.map((ws) => (
                <Button
                  key={ws}
                  color={ws === workspace ? "primary" : "inherit"}
                  size="small"
                  variant={ws === workspace ? "contained" : "text"}
                  onClick={() => navigate(workspaceHome[ws])}
                >
                  {t(workspaceLabelKey[ws])}
                </Button>
              ))}
            </Stack>
          ) : null}
          <Button color="inherit" onClick={() => logout()} sx={{ flexShrink: 0 }}>
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
                top: APP_BAR_HEIGHT,
                height: `calc(100% - ${APP_BAR_HEIGHT}px)`,
                borderColor: "divider",
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
              "& .MuiDrawer-paper": { width: { xs: "min(100%, 300px)", sm: DRAWER_WIDTH }, boxSizing: "border-box" },
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
          minWidth: 0,
          p: { xs: 1.5, sm: 2.5, md: 3 },
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          mt: `${APP_BAR_HEIGHT}px`,
        }}
      >
        <PageContainer>
          {!isDesktop ? (
            <Stack spacing={1} sx={{ mb: 2 }}>
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                {availableWorkspaces.map((ws) => (
                  <Button
                    key={ws}
                    size="small"
                    variant={ws === workspace ? "contained" : "outlined"}
                    onClick={() => navigate(workspaceHome[ws])}
                    sx={{ flex: { xs: "1 1 calc(50% - 8px)", sm: "0 1 auto" }, minHeight: 40 }}
                  >
                    {t(workspaceLabelKey[ws])}
                  </Button>
                ))}
              </Stack>
              <Stack
                direction="row"
                spacing={1}
                sx={{
                  flexWrap: { xs: "nowrap", sm: "wrap" },
                  overflowX: { xs: "auto", sm: "visible" },
                  pb: { xs: 0.5, sm: 0 },
                }}
                useFlexGap
              >
                {items.map((item) => (
                  <Button
                    key={item.to}
                    component={NavLink}
                    to={item.to}
                    end={item.to === workspaceHome[workspace]}
                    size="small"
                    variant="outlined"
                    sx={{
                      flexShrink: 0,
                      minHeight: 40,
                      "&.active": { bgcolor: "action.selected", borderColor: "primary.main" },
                    }}
                  >
                    {t(item.labelKey)}
                  </Button>
                ))}
              </Stack>
            </Stack>
          ) : null}
          <Outlet />
        </PageContainer>
      </Box>
    </Box>
  );
}
