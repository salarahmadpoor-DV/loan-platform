import { Box, Container, Link, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";

export function PublicFooter() {
  return (
    <Box
      component="footer"
      sx={{
        borderTop: 1,
        borderColor: "divider",
        bgcolor: "background.paper",
        py: { xs: 3, md: 4 },
        mt: "auto",
      }}
    >
      <Container maxWidth="lg">
        <Stack
          direction={{ xs: "column", sm: "row" }}
          spacing={2}
          justifyContent="space-between"
          alignItems={{ xs: "flex-start", sm: "center" }}
        >
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="subtitle1">{t("app.name")}</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 0.75, maxWidth: 480 }}>
              {t("public.footer.tagline")}
            </Typography>
          </Box>
          <Stack
            component="nav"
            direction="row"
            spacing={2}
            useFlexGap
            flexWrap="wrap"
            aria-label={t("app.name")}
          >
            <Link component={RouterLink} to="/#categories" color="text.secondary" underline="hover" variant="body2">
              {t("public.nav.findServices")}
            </Link>
            <Link component={RouterLink} to="/#how-it-works" color="text.secondary" underline="hover" variant="body2">
              {t("public.nav.howItWorks")}
            </Link>
            <Link component={RouterLink} to="/login" color="text.secondary" underline="hover" variant="body2">
              {t("auth.signIn")}
            </Link>
          </Stack>
        </Stack>
      </Container>
    </Box>
  );
}
