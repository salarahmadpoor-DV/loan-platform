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
import { Link as RouterLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { defaultWorkspacePath } from "../../../../shared/auth/workspaces";
import { t } from "../../../../shared/i18n";
import {
  findServicePath,
  professionalJoinPath,
} from "../../../../shared/marketplace/publicPaths";

const NAV_LINKS = [
  { href: "/#categories", labelKey: "public.nav.findServices" as const },
  { href: "/#how-it-works", labelKey: "public.nav.howItWorks" as const },
  { href: "/#for-professionals", labelKey: "public.nav.forProfessionals" as const },
];

export function PublicHeader() {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up("md"));
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const findPath = findServicePath(isAuthenticated, user?.roles);
  const joinPath = professionalJoinPath(isAuthenticated, user?.roles);
  const workspacePath = defaultWorkspacePath(user?.roles);
  const drawerAnchor = theme.direction === "rtl" ? "right" : "left";

  function go(path: string) {
    setOpen(false);
    navigate(path);
  }

  const actions = isAuthenticated ? (
    <Button variant="contained" onClick={() => go(workspacePath === "/" ? findPath : workspacePath)}>
      {t("public.nav.workspace")}
    </Button>
  ) : (
    <>
      <Button color="inherit" onClick={() => go("/login")}>
        {t("auth.signIn")}
      </Button>
      <Button variant="contained" onClick={() => go(findPath)}>
        {t("public.nav.getStarted")}
      </Button>
    </>
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
      }}
    >
      <Toolbar sx={{ gap: 1, px: { xs: 1.5, sm: 3 } }}>
        {!isDesktop ? (
          <IconButton color="inherit" aria-label={t("nav.openMenu")} onClick={() => setOpen(true)}>
            <Typography component="span" fontWeight={700} aria-hidden>
              ≡
            </Typography>
          </IconButton>
        ) : null}
        <Typography
          variant="h6"
          component={RouterLink}
          to="/"
          sx={{ color: "inherit", textDecoration: "none", fontWeight: 700, flexGrow: { xs: 1, md: 0 } }}
        >
          {t("app.name")}
        </Typography>
        {isDesktop ? (
          <Stack direction="row" spacing={0.5} sx={{ flexGrow: 1, px: 2 }} component="nav" aria-label={t("app.name")}>
            {NAV_LINKS.map((link) => (
              <Button key={link.href} color="inherit" href={link.href}>
                {t(link.labelKey)}
              </Button>
            ))}
          </Stack>
        ) : (
          <Box sx={{ flexGrow: 1 }} />
        )}
        <Stack direction="row" spacing={1} sx={{ flexShrink: 0 }}>
          {isDesktop ? actions : (
            <Button variant="contained" size="small" onClick={() => go(isAuthenticated ? workspacePath : findPath)}>
              {isAuthenticated ? t("public.nav.workspace") : t("public.nav.getStarted")}
            </Button>
          )}
        </Stack>
      </Toolbar>
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
              <ListItemButton key={link.href} href={link.href} onClick={() => setOpen(false)}>
                <ListItemText primary={t(link.labelKey)} />
              </ListItemButton>
            ))}
            {!isAuthenticated ? (
              <ListItemButton onClick={() => go("/login")}>
                <ListItemText primary={t("auth.signIn")} />
              </ListItemButton>
            ) : null}
            <ListItemButton
              onClick={() => go(isAuthenticated ? (workspacePath === "/" ? findPath : workspacePath) : findPath)}
            >
              <ListItemText
                primary={isAuthenticated ? t("public.nav.workspace") : t("public.nav.getStarted")}
              />
            </ListItemButton>
            <ListItemButton onClick={() => go(joinPath)}>
              <ListItemText primary={t("public.nav.forProfessionals")} />
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
