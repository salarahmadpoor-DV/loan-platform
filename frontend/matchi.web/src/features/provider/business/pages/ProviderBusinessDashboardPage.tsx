import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";
import { OwnedBusinessPage } from "../components/OwnedBusinessPage";
import { useOwnedBusinessTeam } from "../hooks/useOwnedBusinessTeam";

function DashboardBody({ business }: { business: MyBusinessProfile }) {
  const team = useOwnedBusinessTeam(business.id);
  const members = team.data ?? [];
  const activeCount = members.filter((item) => item.status.toLowerCase() === "active").length;
  const pendingCount = members.filter((item) => item.status.toLowerCase() === "pending").length;

  return (
    <Stack spacing={2}>
      <AppCard>
        <Stack spacing={1.25}>
          <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap alignItems="center">
            <Typography variant="h6">{business.name}</Typography>
            <StatusChip label={business.status} tone="info" />
          </Stack>
          {business.description?.trim() ? (
            <Typography variant="body1" sx={{ whiteSpace: "pre-wrap" }}>
              {business.description}
            </Typography>
          ) : null}
          <Typography variant="body2" color="text.secondary">
            {t("provider.business.rating", { rating: business.rating })}
            {" · "}
            {t("provider.business.reviews", { count: business.reviewCount })}
            {" · "}
            {t("provider.business.completedJobs", { count: business.completedJobCount })}
          </Typography>
          {team.data ? (
            <Typography variant="caption" color="text.secondary">
              {t("provider.business.memberCount", { count: members.length })}
              {" · "}
              {t("provider.business.activeCount", { count: activeCount })}
              {" · "}
              {t("provider.business.pendingCount", { count: pendingCount })}
            </Typography>
          ) : null}
        </Stack>
      </AppCard>
      {team.isPending ? <LoadingState /> : null}
      {team.isError ? <ErrorAlert error={team.error} /> : null}
      {!team.isPending && !team.isError && members.length === 0 ? (
        <EmptyState title={t("provider.business.emptyTeam")} />
      ) : null}
    </Stack>
  );
}

export function ProviderBusinessDashboardPage() {
  return (
    <OwnedBusinessPage
      title={t("provider.business.dashboardTitle")}
      description={t("provider.business.dashboardDescription")}
      action={
        <Button component={RouterLink} to="/provider/business/create" variant="contained">
          {t("provider.business.create")}
        </Button>
      }
    >
      {(business) => <DashboardBody business={business} />}
    </OwnedBusinessPage>
  );
}
