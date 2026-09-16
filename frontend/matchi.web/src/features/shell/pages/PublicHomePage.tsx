import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../shared/i18n";
import { APP_WORKSPACES, workspaceHome, workspaceLabelKey } from "../../../shared/navigation/navModel";
import { AppCard } from "../../../shared/ui/AppCard";
import { PageHeader } from "../../../shared/ui/PageHeader";

export function PublicHomePage() {
  return (
    <Stack spacing={{ xs: 3, md: 4 }}>
      <PageHeader title={t("app.name")} description={t("public.homeDescription")} />
      <AppCard>
        <Typography variant="h6" component="h2" sx={{ mb: 1 }}>
          {t("auth.signIn")}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {t("public.signedInWorkspaces")}
        </Typography>
        <Button
          component={RouterLink}
          to="/login"
          variant="contained"
          size="large"
          sx={{ width: { xs: "100%", sm: "auto" } }}
        >
          {t("auth.signIn")}
        </Button>
      </AppCard>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        useFlexGap
        sx={{ flexWrap: "wrap" }}
      >
        {APP_WORKSPACES.map((ws) => (
          <Box key={ws} sx={{ flex: { sm: "1 1 240px" }, minWidth: 0 }}>
            <AppCard>
              <Stack spacing={1.5} sx={{ minHeight: { sm: 120 }, justifyContent: "space-between" }}>
                <Typography variant="subtitle1">{t(workspaceLabelKey[ws])}</Typography>
                <Button
                  component={RouterLink}
                  to={workspaceHome[ws]}
                  variant="outlined"
                  sx={{ alignSelf: { xs: "stretch", sm: "flex-start" } }}
                >
                  {t(workspaceLabelKey[ws])}
                </Button>
              </Stack>
            </AppCard>
          </Box>
        ))}
      </Stack>
    </Stack>
  );
}
