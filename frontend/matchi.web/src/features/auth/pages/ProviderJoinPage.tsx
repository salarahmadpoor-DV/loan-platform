import { Box, Container, Stack, Typography } from "@mui/material";
import { Navigate, useNavigate } from "react-router-dom";
import { matchiRadius, matchiShadows } from "../../../app/designTokens";
import { useAuth } from "../../../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../../../shared/auth/useWorkspaceAccess";
import { t } from "../../../shared/i18n";
import {
  professionalJoinPath,
  providerOnboardPath,
} from "../../../shared/marketplace/publicPaths";
import { AppCard } from "../../../shared/ui/AppCard";
import { LoadingState } from "../../../shared/ui/LoadingState";
import { OtpLoginForm } from "../components/OtpLoginForm";

const BENEFITS = [
  "providerJoin.benefit1",
  "providerJoin.benefit2",
  "providerJoin.benefit3",
  "providerJoin.benefit4",
] as const;

export function ProviderJoinPage() {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const access = useWorkspaceAccess();

  if (isAuthenticated) {
    if (!access.isReady) {
      return <LoadingState />;
    }
    return (
      <Navigate to={professionalJoinPath(true, user?.roles, access.capabilities)} replace />
    );
  }

  return (
    <Box
      sx={{
        bgcolor: "background.default",
        py: { xs: 4, md: 8 },
        overflowX: "hidden",
      }}
    >
      <Container maxWidth="lg" sx={{ px: { xs: 2, sm: 3 } }}>
        <Box
          sx={{
            display: "grid",
            gap: { xs: 4, md: 6 },
            alignItems: "start",
            gridTemplateColumns: { xs: "1fr", md: "minmax(0, 1.1fr) minmax(0, 0.9fr)" },
          }}
        >
          <Stack spacing={2.5} sx={{ order: { xs: 2, md: 1 }, minWidth: 0 }}>
            <Typography variant="h1" component="h1">
              {t("providerJoin.title")}
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 520 }}>
              {t("providerJoin.description")}
            </Typography>
            <Box component="ul" sx={{ m: 0, pl: 2.5, display: "grid", gap: 1.25 }}>
              {BENEFITS.map((key) => (
                <Typography key={key} component="li" variant="body1">
                  {t(key)}
                </Typography>
              ))}
            </Box>
          </Stack>
          <Box sx={{ order: { xs: 1, md: 2 }, minWidth: 0 }}>
            <AppCard
              sx={{
                borderRadius: `${matchiRadius.lg}px`,
                boxShadow: matchiShadows.elevated,
                borderColor: "transparent",
              }}
            >
              <OtpLoginForm
                headingComponent="h2"
                title={t("providerJoin.cardTitle")}
                description={t("providerJoin.cardDescription")}
                mobileSubmitLabel={t("providerJoin.cta")}
                onLoggedIn={() => navigate(providerOnboardPath, { replace: true })}
              />
            </AppCard>
          </Box>
        </Box>
      </Container>
    </Box>
  );
}
