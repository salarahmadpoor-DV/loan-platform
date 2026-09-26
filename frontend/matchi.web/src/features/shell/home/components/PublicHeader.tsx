import {
  AppBar,
  Box,
  Button,
  Container,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Menu,
  MenuItem,
  Stack,
  Toolbar,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useEffect, useId, useState, type MouseEvent } from "react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { matchiShadows } from "../../../../app/designTokens";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../../../../shared/auth/useWorkspaceAccess";
import { getLocale, t } from "../../../../shared/i18n";
import { changeAppLocale } from "../../../../shared/i18n/LocaleProvider";
import {
  customerCreateRequestPath,
  customerDashboardPath,
  findServicePath,
  providerJoinPath,
} from "../../../../shared/marketplace/publicPaths";
import { usePublicEntry } from "../../../auth/PublicEntryContext";

const NAV_LINKS = [
  { hash: "categories", labelKey: "public.nav.findServices" as const },
  { hash: "how-it-works", labelKey: "public.nav.howItWorks" as const },
];

function scrollToSection(hash: string) {
  const el = document.getElementById(hash);
  el?.scrollIntoView({ behavior: "smooth", block: "start" });
}

export function PublicHeader() {
  const theme = useTheme();
  const compactNav = !useMediaQuery(theme.breakpoints.up("md"));
  const [open, setOpen] = useState(false);
  const [elevated, setElevated] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, user } = useAuth();
  const { defaultPath, capabilities } = useWorkspaceAccess();
  const { openCustomerLogin } = usePublicEntry();
  const findPath = findServicePath(isAuthenticated, user?.roles, undefined, capabilities);
  const drawerAnchor = theme.direction === "rtl" ? "right" : "left";
  const menuId = useId();
  const authMenuId = useId();
  const [authMenuEl, setAuthMenuEl] = useState<HTMLElement | null>(null);
  const primaryPath = isAuthenticated ? (defaultPath === "/" ? findPath : defaultPath) : findPath;
  const locale = getLocale();

  useEffect(() => {
    const onScroll = () => setElevated(window.scrollY > 8);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  function go(path: string) {
    setOpen(false);
    setAuthMenuEl(null);
    navigate(path);
  }

  function goSection(hash: string) {
    setOpen(false);
    if (location.pathname !== "/") {
      navigate({ pathname: "/", hash: `#${hash}` });
      return;
    }
    scrollToSection(hash);
  }

  function openAuthMenu(event: MouseEvent<HTMLElement>) {
    setAuthMenuEl(event.currentTarget);
  }

  function closeAuthMenu() {
    setAuthMenuEl(null);
  }

  function chooseCustomer() {
    setOpen(false);
    closeAuthMenu();
    openCustomerLogin(customerDashboardPath);
  }

  function chooseRequestService() {
    setOpen(false);
    closeAuthMenu();
    openCustomerLogin(customerCreateRequestPath);
  }

  function chooseProvider() {
    go(providerJoinPath);
  }

  useEffect(() => {
    if (location.pathname === "/" && location.hash.length > 1) {
      const id = location.hash.slice(1);
      requestAnimationFrame(() => scrollToSection(id));
    }
  }, [location.pathname, location.hash]);

  const localeToggle = (
    <Button
      color="inherit"
      onClick={() => changeAppLocale(locale === "fa-IR" ? "en-US" : "fa-IR")}
      aria-label={t("locale.switch")}
    >
      {locale === "fa-IR" ? t("locale.en") : t("locale.fa")}
    </Button>
  );

  return (
    <AppBar
      position="sticky"
      elevation={0}
      color="inherit"
      sx={{
        borderBottom: 1,
        borderColor: "divider",
        bgcolor: "background.paper",
        color: "text.primary",
        boxShadow: elevated ? matchiShadows.header : "none",
      }}
    >
      <Container maxWidth="lg" disableGutters>
        <Toolbar
          disableGutters
          sx={{
            display: "grid",
            gridTemplateColumns: "auto minmax(0, 1fr) auto",
            columnGap: { xs: 1, sm: 2 },
            alignItems: "center",
            px: { xs: 1.5, sm: 3 },
            minHeight: { xs: 64, lg: 72 },
            width: "100%",
          }}
        >
          <Stack direction="row" spacing={1} alignItems="center" sx={{ minWidth: 0 }}>
            {compactNav ? (
              <IconButton
                color="inherit"
                aria-label={t("nav.openMenu")}
                aria-expanded={open}
                aria-controls={menuId}
                onClick={() => setOpen(true)}
              >
                <Typography component="span" fontWeight={700} aria-hidden>
                  ≡
                </Typography>
              </IconButton>
            ) : null}
            <Stack
              component={RouterLink}
              to="/"
              direction="row"
              spacing={1}
              alignItems="center"
              sx={{ color: "inherit", textDecoration: "none", minWidth: 0 }}
            >
              <Box
                aria-hidden
                sx={{
                  width: 28,
                  height: 28,
                  borderRadius: 1,
                  bgcolor: "primary.main",
                  flexShrink: 0,
                }}
              />
              <Typography variant="h6" component="span" noWrap sx={{ fontWeight: 700 }}>
                {t("app.name")}
              </Typography>
            </Stack>
          </Stack>
          {!compactNav ? (
            <Stack
              direction="row"
              spacing={0.5}
              sx={{ minWidth: 0, justifyContent: "flex-start" }}
              component="nav"
              aria-label={t("public.nav.findServices")}
            >
              {NAV_LINKS.map((link) => (
                <Button key={link.hash} color="inherit" onClick={() => goSection(link.hash)}>
                  {t(link.labelKey)}
                </Button>
              ))}
            </Stack>
          ) : (
            <Box />
          )}
          <Stack direction="row" spacing={1} sx={{ alignItems: "center", justifyContent: "flex-end" }}>
            {localeToggle}
            {!isAuthenticated && !compactNav ? (
              <>
                <Button
                  color="inherit"
                  aria-haspopup="menu"
                  aria-expanded={Boolean(authMenuEl)}
                  aria-controls={authMenuEl ? authMenuId : undefined}
                  onClick={openAuthMenu}
                >
                  {t("auth.chooseRole")}
                </Button>
                <Menu
                  id={authMenuId}
                  anchorEl={authMenuEl}
                  open={Boolean(authMenuEl)}
                  onClose={closeAuthMenu}
                  MenuListProps={{ "aria-label": t("auth.chooseRole") }}
                  slotProps={{
                    paper: {
                      sx: { minWidth: 280, maxWidth: 360 },
                    },
                  }}
                >
                  <MenuItem onClick={chooseCustomer} sx={{ whiteSpace: "normal", alignItems: "flex-start" }}>
                    <ListItemText
                      primary={t("auth.customer.title")}
                      secondary={t("auth.customer.description")}
                    />
                  </MenuItem>
                  <MenuItem onClick={chooseProvider} sx={{ whiteSpace: "normal", alignItems: "flex-start" }}>
                    <ListItemText
                      primary={t("auth.provider.title")}
                      secondary={t("auth.provider.description")}
                    />
                  </MenuItem>
                </Menu>
              </>
            ) : null}
            <Button
              variant="contained"
              onClick={() => {
                if (isAuthenticated) {
                  go(primaryPath);
                  return;
                }
                chooseRequestService();
              }}
            >
              {isAuthenticated ? t("public.nav.workspace") : t("public.hero.requestService")}
            </Button>
          </Stack>
        </Toolbar>
      </Container>
      <Drawer
        anchor={drawerAnchor}
        open={open}
        onClose={() => setOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{ "& .MuiDrawer-paper": { width: { xs: "min(100%, 300px)" } } }}
      >
        <Box id={menuId} sx={{ p: 2 }} role="presentation">
          <Typography variant="subtitle1" sx={{ mb: 1 }}>
            {t("app.name")}
          </Typography>
          <List>
            {NAV_LINKS.map((link) => (
              <ListItemButton key={link.hash} onClick={() => goSection(link.hash)}>
                <ListItemText primary={t(link.labelKey)} />
              </ListItemButton>
            ))}
            <ListItemButton onClick={() => goSection("for-professionals")}>
              <ListItemText primary={t("public.nav.forProfessionals")} />
            </ListItemButton>
            {!isAuthenticated ? (
              <>
                <ListItemButton onClick={chooseCustomer}>
                  <ListItemText
                    primary={t("auth.customer.title")}
                    secondary={t("auth.customer.description")}
                  />
                </ListItemButton>
                <ListItemButton onClick={chooseProvider}>
                  <ListItemText
                    primary={t("auth.provider.title")}
                    secondary={t("auth.provider.description")}
                  />
                </ListItemButton>
              </>
            ) : null}
            <ListItemButton
              onClick={() => {
                if (isAuthenticated) {
                  go(primaryPath);
                  return;
                }
                chooseRequestService();
              }}
            >
              <ListItemText
                primary={isAuthenticated ? t("public.nav.workspace") : t("public.hero.requestService")}
              />
            </ListItemButton>
            <ListItemButton
              onClick={() => {
                changeAppLocale(locale === "fa-IR" ? "en-US" : "fa-IR");
                setOpen(false);
              }}
            >
              <ListItemText primary={locale === "fa-IR" ? t("locale.en") : t("locale.fa")} />
            </ListItemButton>
          </List>
          <Button fullWidth onClick={() => setOpen(false)}>
            {t("public.nav.closeMenu")}
          </Button>
        </Box>
      </Drawer>
    </AppBar>
  );
}
