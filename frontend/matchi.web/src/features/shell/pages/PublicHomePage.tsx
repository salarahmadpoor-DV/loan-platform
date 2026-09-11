import { Stack, Typography } from "@mui/material";
import Link from "@mui/material/Link";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../shared/i18n";
import { APP_WORKSPACES, workspaceHome, workspaceLabelKey } from "../../../shared/navigation/navModel";
import { PageHeader } from "../../../shared/ui/PageHeader";

export function PublicHomePage() {
  return (
    <>
      <PageHeader title={t("app.name")} description={t("public.homeDescription")} />
      <Typography color="text.secondary" paragraph>
        {t("public.signedInWorkspaces")}
      </Typography>
      <Stack spacing={1}>
        <Link component={RouterLink} to="/login">
          {t("auth.signIn")}
        </Link>
        {APP_WORKSPACES.map((ws) => (
          <Link key={ws} component={RouterLink} to={workspaceHome[ws]}>
            {t(workspaceLabelKey[ws])}
          </Link>
        ))}
      </Stack>
    </>
  );
}
