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
  Stack,
  Toolbar,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useEffect, useState } from "react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { matchiShadows } from "../../../../app/designTokens";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { defaultWorkspacePath } from "../../../../shared/auth/workspaces";
import { t } from "../../../../shared/i18n";
import { findServicePath } from "../../../../shared/marketplace/publicPaths";

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
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [open, setOpen] = useState(false);
  const [elevated, setElevated] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, user } = useAuth();
  const findPath = findServicePath(isAuthenticated, user?.roles);
  const workspacePath = defaultWorkspacePath(user?.roles);
  const drawerAnchor = theme.direction === "rtl" ? "right" : "left";
  const primaryPath = isAuthenticated
    ? workspacePath === "/"
      ? findPath
      : workspacePath
    : findPath;

  useEffect(() => {
    const onScroll = () => setElevated(window.scrollY > 8);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  function go(path: string) {
    setOpen(false);
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

  useEffect(() => {
    if (location.pathname === "/" && location.hash.length > 1) {
      const id = location.hash.slice(1);
      requestAnimationFrame(() => scrollToSection(id));
    }
  }, [location.pathname, location.hash]);

  const actions = (
    <Stack direction="row" spacing={1} sx={{ flexShrink: 0, alignItems: "center" }}>
      {!isAuthenticated ? (
        <Button color="inherit" onClick={() => go("/login")}>
          {t("auth.signIn")}
        </Button>
      ) : null}
      <Button variant="contained" onClick={() => go(primaryPath)}>
        {isAuthenticated ? t("public.nav.workspace") : t("public.nav.getStarted")}
      </Button>
    </Stack>
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
        <Toolbar sx={{ gap: 1, px: { xs: 1.5, sm: 3 } }}>
          {!isDesktop ? (
            <IconButton color="inherit" aria-label={t("nav.openMenu")} onClick={() => setOpen(true)}>
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
            sx={{ color: "inherit", textDecoration: "none", flexGrow: { xs: 1, md: 0 }, minWidth: 0 }}
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
          {isDesktop ? (
            <Stack
              direction="row"
              spacing={0.5}
              sx={{ flexGrow: 1, px: 2 }}
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
            <Box sx={{ flexGrow: 1 }} />
          )}
          {actions}
        </Toolbar>
      </Container>
      <Drawer
        anchor={drawerAnchor}
        open={open}
        onClose={() => setOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{ "& .MuiDrawer-paper": { width: { xs: "min(100%, 300px)" } } }}
      >
        <Box sx={{ p: 2 }} role="presentation">
          <Typography variant="subtitle1" sx={{ mb: 1 }}>
            {t("app.name")}
          </Typography>
          <List>
            {NAV_LINKS.map((link) => (
              <ListItemButton key={link.hash} onClick={() => goSection(link.hash)}>
                <ListItemText primary={t(link.labelKey)} />
              </ListItemButton>
            ))}
            {!isAuthenticated ? (
              <ListItemButton onClick={() => go("/login")}>
                <ListItemText primary={t("auth.signIn")} />
              </ListItemButton>
            ) : null}
            <ListItemButton onClick={() => go(primaryPath)}>
              <ListItemText
                primary={isAuthenticated ? t("public.nav.workspace") : t("public.nav.getStarted")}
              />
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
