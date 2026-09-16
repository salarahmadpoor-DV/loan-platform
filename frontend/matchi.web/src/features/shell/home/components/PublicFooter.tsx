import { Box, Container, Link, Typography } from "@mui/material";
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
        py: 4,
        mt: "auto",
      }}
    >
      <Container maxWidth="lg">
        <Typography variant="subtitle1">{t("app.name")}</Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1, maxWidth: 520 }}>
          {t("public.footer.tagline")}
        </Typography>
        <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 2 }}>
          <Link component={RouterLink} to="/" color="inherit" underline="hover">
            {t("app.name")}
          </Link>
          {" · "}
          <Link component={RouterLink} to="/login" color="inherit" underline="hover">
            {t("auth.signIn")}
          </Link>
        </Typography>
      </Container>
    </Box>
  );
}
