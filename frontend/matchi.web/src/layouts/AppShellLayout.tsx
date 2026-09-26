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
import { t, getLocale } from "../shared/i18n";
import { changeAppLocale } from "../shared/i18n/LocaleProvider";
import { useAuth } from "../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../shared/auth/useWorkspaceAccess";
import {
  workspaceHome,
  workspaceLabelKey,
  workspaceNav,
  providerOwnedBusinessNav,
  providerBusinessHomeNav,
  type AppWorkspace,
} from "../shared/navigation/navModel";
import { PageContainer } from "../shared/ui/PageContainer";

const DRAWER_WIDTH = 260;
const APP_BAR_HEIGHT = 64;

type AppShellLayoutProps = {
  workspace: AppWorkspace;
};

export function AppShellLayout({ workspace }: AppShellLayoutProps) {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { workspaces: availableWorkspaces, canAccess } = useWorkspaceAccess();
  const items =
    workspace === "provider"
      ? [
          ...workspaceNav.provider,
          ...(canAccess("business") ? providerOwnedBusinessNav : providerBusinessHomeNav),
        ]
      : workspaceNav[workspace];
  const menuId = "workspace-nav";
  const locale = getLocale();

  const drawer = (
    <Box sx={{ pt: 1, px: 0.5 }} onClick={() => setMobileOpen(false)}>
      <Typography
        variant="overline"
        color={workspace === "provider" ? "secondary.main" : "text.secondary"}
        sx={{ px: 2, py: 1, display: "block" }}
      >
        {t("workspace.shell", { name: t(workspaceLabelKey[workspace]) })}
      </Typography>
      <List disablePadding>
        {items.map((item) => (
          <Box key={item.to}>
            {item.to === "/provider/business" ? (
              <Typography variant="overline" color="text.secondary" sx={{ px: 2, pt: 2, display: "block" }}>
                {t("provider.business.navSection")}
              </Typography>
            ) : null}
            <ListItemButton
              component={NavLink}
              to={item.to}
              end={item.end ?? item.to === workspaceHome[workspace]}
              sx={{
                mx: 1,
                borderRadius: 1,
                minHeight: 48,
                "&.active": {
                  bgcolor: "action.selected",
                  color: "primary.main",
                  fontWeight: 700,
                  borderInlineStart: 3,
                  borderColor: "primary.main",
                },
              }}
            >
              <ListItemText
                primary={t(item.labelKey)}
                primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
              />
            </ListItemButton>
          </Box>
        ))}
      </List>
    </Box>
  );

  return (
    <Box sx={{ minHeight: "100vh", minWidth: 0, width: "100%" }}>
      <AppBar
        position="fixed"
        elevation={0}
        color="inherit"
        sx={{
          zIndex: (t) => t.zIndex.drawer + 1,
          borderBottom: 3,
          borderColor:
            workspace === "provider"
              ? "secondary.main"
              : workspace === "business"
                ? "warning.main"
                : "divider",
          bgcolor: "background.paper",
          color: "text.primary",
        }}
      >
        <Toolbar
          sx={{
            display: "grid",
            gridTemplateColumns: "auto minmax(0, 1fr) auto",
            columnGap: { xs: 1, sm: 2 },
            alignItems: "center",
            px: { xs: 1, sm: 2 },
            minHeight: APP_BAR_HEIGHT,
            width: "100%",
          }}
        >
          <Stack direction="row" spacing={1} alignItems="center" sx={{ minWidth: 0 }}>
            {!isDesktop ? (
              <IconButton
                color="inherit"
                aria-label={t("nav.openMenu")}
                aria-expanded={mobileOpen}
                aria-controls={menuId}
                onClick={() => setMobileOpen(true)}
              >
                <Typography component="span" fontWeight={700} aria-hidden>
                  ≡
                </Typography>
              </IconButton>
            ) : null}
            <Stack spacing={0} sx={{ minWidth: 0 }}>
              <Typography variant="subtitle1" component="p" noWrap>
                {t("app.name")}
              </Typography>
              <Typography
                variant="caption"
                color={workspace === "provider" ? "secondary.main" : "text.secondary"}
                noWrap
              >
                {t("workspace.shell", { name: t(workspaceLabelKey[workspace]) })}
              </Typography>
            </Stack>
          </Stack>
          {isDesktop ? (
            <Stack
              direction="row"
              spacing={0.5}
              sx={{ minWidth: 0, justifyContent: "flex-start", flexWrap: "nowrap", overflow: "hidden" }}
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
          ) : (
            <Box />
          )}
          <Stack direction="row" spacing={1} sx={{ alignItems: "center", justifyContent: "flex-end" }}>
            <Button
              color="inherit"
              onClick={() => changeAppLocale(locale === "fa-IR" ? "en-US" : "fa-IR")}
              sx={{ flexShrink: 0 }}
              aria-label={t("locale.switch")}
            >
              {locale === "fa-IR" ? t("locale.en") : t("locale.fa")}
            </Button>
            <Button color="inherit" onClick={() => logout()} sx={{ flexShrink: 0 }}>
              {t("auth.signOut")}
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>

      {!isDesktop ? (
        <Drawer
          variant="temporary"
          open={mobileOpen}
          anchor={theme.direction === "rtl" ? "right" : "left"}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            "& .MuiDrawer-paper": {
              width: { xs: "min(100%, 300px)", sm: DRAWER_WIDTH },
              boxSizing: "border-box",
            },
          }}
        >
          <Box id={menuId}>{drawer}</Box>
        </Drawer>
      ) : null}

      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: {
            xs: "minmax(0, 1fr)",
            md: `${DRAWER_WIDTH}px minmax(0, 1fr)`,
          },
          width: "100%",
          minWidth: 0,
          minHeight: "100vh",
          boxSizing: "border-box",
          pt: `${APP_BAR_HEIGHT}px`,
        }}
      >
        {isDesktop ? (
          <Box
            component="nav"
            aria-label={t("workspace.shell", { name: t(workspaceLabelKey[workspace]) })}
            sx={{
              minWidth: 0,
              borderInlineEnd: 1,
              borderColor: "divider",
              bgcolor: "background.paper",
              position: "sticky",
              top: APP_BAR_HEIGHT,
              alignSelf: "start",
              height: `calc(100vh - ${APP_BAR_HEIGHT}px)`,
              overflowY: "auto",
            }}
          >
            {drawer}
          </Box>
        ) : null}

        <Box
          component="main"
          sx={{
            minWidth: 0,
            width: "100%",
            maxWidth: "100%",
            p: { xs: 1.5, sm: 2.5, md: 3 },
            boxSizing: "border-box",
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
                      end={item.end ?? item.to === workspaceHome[workspace]}
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
            <Box sx={{ minWidth: 0 }}>
              <Outlet />
            </Box>
          </PageContainer>
        </Box>
      </Box>
    </Box>
  );
}
