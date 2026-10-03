import {
  AppBar,
  Box,
  BottomNavigation,
  BottomNavigationAction,
  Drawer,
  IconButton,
  Menu as AccountMenu,
  MenuItem,
  Stack,
  Toolbar,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useMemo, useState, type MouseEvent } from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { t, getLocale } from "../shared/i18n";
import { changeAppLocale } from "../shared/i18n/LocaleProvider";
import { useAuth } from "../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../shared/auth/useWorkspaceAccess";
import {
  buildWorkspaceDrawerNav,
  isFocusedTaskPath,
  navItemActive,
  workspaceHome,
  workspaceLabelKey,
  workspacePrimaryNav,
  type AppWorkspace,
} from "../shared/navigation/navModel";
import { PageContainer } from "../shared/ui/PageContainer";
import { BackIcon, Menu, NavIcon, PersonOutline } from "../shared/ui/icons";
import { mobileDrawerAnchor, mobileDrawerPaperSx } from "./mobileDrawerPlacement";
import { WorkspaceDrawerNav } from "./WorkspaceDrawerNav";
import { NotificationBell } from "../features/notifications/components/NotificationBell";
import { useWorkspaceNavigation } from "../features/workspace/hooks/useWorkspaceNavigation";

const DRAWER_WIDTH = 260;
const APP_BAR_HEIGHT = 56;
const BOTTOM_NAV_HEIGHT = 56;

type AppShellLayoutProps = {
  workspace: AppWorkspace;
};

function focusedBackPath(pathname: string, workspace: AppWorkspace): string {
  const proposal = pathname.match(/^\/provider\/requests\/([^/]+)\/proposal$/);
  if (proposal) {
    return `/provider/requests/${proposal[1]}`;
  }
  return workspaceHome[workspace];
}

export function AppShellLayout({ workspace }: AppShellLayoutProps) {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [mobileOpen, setMobileOpen] = useState(false);
  const [accountEl, setAccountEl] = useState<HTMLElement | null>(null);
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const { logout } = useAuth();
  const { workspaces: availableWorkspaces, canAccess, preferredWorkspace } = useWorkspaceAccess();
  const { switchTo, setPreferred } = useWorkspaceNavigation();
  const drawerModel = useMemo(
    () => buildWorkspaceDrawerNav(workspace, canAccess("business")),
    [workspace, canAccess],
  );
  const primaryItems = workspacePrimaryNav[workspace];
  const menuId = "workspace-nav";
  const locale = getLocale();
  const focused = !isDesktop && isFocusedTaskPath(pathname);
  const drawerAnchor = mobileDrawerAnchor(theme.direction);
  const home = workspaceHome[workspace];
  const primaryMatch = primaryItems.find((item) => navItemActive(item, pathname, home));
  const bottomValue = mobileOpen ? "more" : (primaryMatch?.to ?? "more");

  const closeDrawer = () => setMobileOpen(false);

  const drawerNav = (
    <WorkspaceDrawerNav
      workspace={workspace}
      home={drawerModel.home}
      groups={drawerModel.groups}
      pathname={pathname}
      direction={theme.direction === "rtl" ? "rtl" : "ltr"}
      availableWorkspaces={availableWorkspaces}
      showSessionTools={!isDesktop}
      localeLabel={locale === "fa-IR" ? t("locale.en") : t("locale.fa")}
      onToggleLocale={() => changeAppLocale(locale === "fa-IR" ? "en-US" : "fa-IR")}
      onLogout={() => logout()}
      onSwitchWorkspace={(ws) => {
        void switchTo(ws);
      }}
      onSetPreferredWorkspace={(ws) => {
        void setPreferred(ws);
      }}
      preferredWorkspace={preferredWorkspace}
      onNavigate={isDesktop ? undefined : closeDrawer}
    />
  );

  return (
    <Box sx={{ minHeight: "100vh", minWidth: 0, width: "100%" }}>
      <AppBar
        position="fixed"
        elevation={0}
        color="inherit"
        sx={{
          zIndex: (z) => z.zIndex.drawer + 1,
          borderBottom: 1,
          borderColor: "divider",
          bgcolor: "background.paper",
          color: "text.primary",
        }}
      >
        <Toolbar
          dir={theme.direction}
          style={{ display: "grid", gridTemplateColumns: "auto minmax(0, 1fr) auto" }}
          sx={{
            columnGap: { xs: 0.5, sm: 2 },
            alignItems: "center",
            px: { xs: 0.5, sm: 2 },
            minHeight: APP_BAR_HEIGHT,
            width: "100%",
          }}
        >
          <Stack direction="row" spacing={0.5} alignItems="center" sx={{ minWidth: 0 }} style={{ flexDirection: "row" }}>
            {focused ? (
              <IconButton
                color="inherit"
                aria-label={t("request.create.back")}
                onClick={() => navigate(focusedBackPath(pathname, workspace))}
              >
                <BackIcon aria-hidden />
              </IconButton>
            ) : !isDesktop ? (
              <IconButton
                color="inherit"
                aria-label={t("nav.openMenu")}
                aria-expanded={mobileOpen}
                aria-controls={menuId}
                onClick={() => setMobileOpen(true)}
              >
                <Menu aria-hidden />
              </IconButton>
            ) : null}
          </Stack>
          <Stack spacing={0} sx={{ minWidth: 0, alignItems: "center", textAlign: "center" }}>
            <Typography variant="subtitle1" component="p" noWrap fontWeight={700}>
              {focused
                ? pathname.startsWith("/provider/")
                  ? t("provider.proposalCreate.title")
                  : t("request.create.pageTitle")
                : t("app.name")}
            </Typography>
            {!focused ? (
              <Typography
                variant="caption"
                color={workspace === "provider" ? "secondary.main" : "text.secondary"}
                noWrap
              >
                {t(workspaceLabelKey[workspace])}
              </Typography>
            ) : null}
          </Stack>
          {isDesktop ? (
            <Stack
              direction="row"
              spacing={0.25}
              alignItems="center"
              justifyContent="flex-end"
              style={{ flexDirection: "row" }}
              sx={{ minWidth: 0 }}
            >
              <NotificationBell workspace={workspace} />
              <IconButton
                color="inherit"
                aria-label={t("nav.account")}
                aria-haspopup="menu"
                onClick={(event: MouseEvent<HTMLElement>) => setAccountEl(event.currentTarget)}
              >
                <PersonOutline aria-hidden />
              </IconButton>
            </Stack>
          ) : focused ? (
            <Box />
          ) : (
            <Stack direction="row" spacing={0} style={{ flexDirection: "row" }}>
              <IconButton
                color="inherit"
                aria-label={t("nav.account")}
                aria-haspopup="menu"
                onClick={(event: MouseEvent<HTMLElement>) => setAccountEl(event.currentTarget)}
              >
                <PersonOutline aria-hidden />
              </IconButton>
            </Stack>
          )}
        </Toolbar>
      </AppBar>

      <AccountMenu anchorEl={accountEl} open={Boolean(accountEl)} onClose={() => setAccountEl(null)}>
        {availableWorkspaces.map((ws) => (
          <MenuItem
            key={ws}
            selected={ws === workspace}
            onClick={() => {
              setAccountEl(null);
              void switchTo(ws);
            }}
          >
            {t(workspaceLabelKey[ws])}
            {preferredWorkspace === ws ? ` · ${t("workspace.default")}` : ""}
          </MenuItem>
        ))}
        {preferredWorkspace !== workspace ? (
          <MenuItem
            onClick={() => {
              setAccountEl(null);
              void setPreferred(workspace);
            }}
          >
            {t("workspace.setDefault")}
          </MenuItem>
        ) : null}
        <MenuItem
          onClick={() => {
            setAccountEl(null);
            changeAppLocale(locale === "fa-IR" ? "en-US" : "fa-IR");
          }}
        >
          {locale === "fa-IR" ? t("locale.en") : t("locale.fa")}
        </MenuItem>
        <MenuItem
          onClick={() => {
            setAccountEl(null);
            logout();
          }}
        >
          {t("auth.signOut")}
        </MenuItem>
      </AccountMenu>

      {!isDesktop && !focused ? (
        <Drawer
          variant="temporary"
          open={mobileOpen}
          anchor={drawerAnchor}
          onClose={closeDrawer}
          ModalProps={{ keepMounted: true }}
          PaperProps={{ dir: theme.direction }}
          sx={{ "& .MuiDrawer-paper": mobileDrawerPaperSx(theme.direction, { xs: "min(100%, 300px)", sm: DRAWER_WIDTH }) }}
        >
          <Box id={menuId}>{drawerNav}</Box>
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
          pb: !isDesktop && !focused ? `calc(${BOTTOM_NAV_HEIGHT}px + env(safe-area-inset-bottom, 0px))` : 0,
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
            {drawerNav}
          </Box>
        ) : null}

        <Box
          component="main"
          sx={{
            minWidth: 0,
            width: "100%",
            maxWidth: "100%",
            p: { xs: focused ? 1.5 : 1.5, sm: 2.5, md: 3 },
            boxSizing: "border-box",
          }}
        >
          <PageContainer>
            <Box sx={{ minWidth: 0 }}>
              <Outlet />
            </Box>
          </PageContainer>
        </Box>
      </Box>

      {!isDesktop && !focused ? (
        <Box
          component="nav"
          sx={{
            position: "fixed",
            bottom: 0,
            insetInline: 0,
            zIndex: (z) => z.zIndex.appBar,
            borderTop: 1,
            borderColor: "divider",
            bgcolor: "background.paper",
            pb: "env(safe-area-inset-bottom, 0px)",
          }}
        >
          <BottomNavigation
            showLabels
            dir={theme.direction}
            style={{ flexDirection: "row" }}
            value={bottomValue}
            onChange={(_, value: string) => {
              if (value === "more") {
                setMobileOpen(true);
                return;
              }
              navigate(value);
            }}
            sx={{ height: BOTTOM_NAV_HEIGHT }}
          >
            {primaryItems.map((item) => (
              <BottomNavigationAction
                key={item.to}
                value={item.to}
                label={t(item.labelKey)}
                icon={<NavIcon name={item.icon ?? "dashboard"} aria-hidden />}
              />
            ))}
            <BottomNavigationAction
              value="more"
              label={t("nav.more")}
              icon={<NavIcon name="more" aria-hidden />}
            />
          </BottomNavigation>
        </Box>
      ) : null}
    </Box>
  );
}
