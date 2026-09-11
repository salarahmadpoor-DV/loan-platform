import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { useMyProviderDeals } from "../../deals/hooks/useMyProviderDeals";
import { useMyProviderExecutions } from "../../executions/hooks/useMyProviderExecutions";
import { useMyProviderProfile } from "../../profile/hooks/useMyProviderProfile";
import { useMyProviderProposals } from "../../proposals/hooks/useMyProviderProposals";
import { useProviderRequestInbox } from "../../requests/hooks/useProviderRequestInbox";

export function ProviderDashboardPage() {
  const profile = useMyProviderProfile();
  const requests = useProviderRequestInbox();
  const proposals = useMyProviderProposals();
  const deals = useMyProviderDeals();
  const executions = useMyProviderExecutions();

  const isPending =
    requests.isPending || proposals.isPending || deals.isPending || executions.isPending;
  const firstError = requests.error ?? proposals.error ?? deals.error ?? executions.error;
  const isError = requests.isError || proposals.isError || deals.isError || executions.isError;

  const requestCount = requests.data?.length ?? 0;
  const proposalCount = proposals.data?.length ?? 0;
  const dealCount = deals.data?.length ?? 0;
  const executionCount = executions.data?.length ?? 0;
  const hasAny = requestCount + proposalCount + dealCount + executionCount > 0;
  const nameSuffix = profile.data?.name ? `، ${profile.data.name}` : "";

  return (
    <>
      <PageHeader title={t("provider.dashboard.title")} />
      <AppCard>
        <Typography variant="h6">{t("provider.dashboard.welcome", { name: nameSuffix })}</Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          {t("provider.dashboard.intro")}
        </Typography>
      </AppCard>

      <Typography variant="subtitle1" sx={{ mt: 3, mb: 1 }}>
        {t("provider.dashboard.summary")}
      </Typography>

      {isPending ? <LoadingState label={t("provider.dashboard.loading")} /> : null}
      {isError ? (
        <Stack spacing={2} sx={{ mb: 2 }}>
          <ErrorAlert error={firstError} />
          <Button
            variant="outlined"
            onClick={() => {
              void profile.refetch();
              void requests.refetch();
              void proposals.refetch();
              void deals.refetch();
              void executions.refetch();
            }}
            sx={{ alignSelf: "flex-start" }}
          >
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}

      {!isPending && !isError && !hasAny ? (
        <EmptyState
          title={t("provider.dashboard.emptyTitle")}
          body={t("provider.dashboard.emptyBody")}
        />
      ) : null}

      {!isPending && !isError && hasAny ? (
        <Box
          sx={{
            display: "grid",
            gap: 2,
            gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
          }}
        >
          <SummaryCard
            label={t("provider.dashboard.inbox")}
            value={requestCount}
            to="/provider/requests"
          />
          <SummaryCard
            label={t("provider.dashboard.proposals")}
            value={proposalCount}
            to="/provider/proposals"
          />
          <SummaryCard
            label={t("provider.dashboard.deals")}
            value={dealCount}
            to="/provider/deals"
          />
          <SummaryCard
            label={t("provider.dashboard.executions")}
            value={executionCount}
            to="/provider/executions"
          />
        </Box>
      ) : null}

      <Typography variant="subtitle1" sx={{ mt: 3, mb: 1 }}>
        {t("provider.dashboard.next")}
      </Typography>
      <EmptyState
        title={t("provider.dashboard.nextTitle")}
        body={t("provider.dashboard.nextBody")}
      />
    </>
  );
}

function SummaryCard({ label, value, to }: { label: string; value: number; to: string }) {
  return (
    <AppCard>
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="h5" sx={{ mt: 0.5 }}>
        {value}
      </Typography>
      <Button component={RouterLink} to={to} size="small" sx={{ mt: 1 }}>
        {label}
      </Button>
    </AppCard>
  );
}
