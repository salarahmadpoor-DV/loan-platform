import type { ReactNode } from "react";
import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { DealStatusChip } from "../../../customer/deals/components/DealStatusChip";
import { ProposalStatusChip } from "../../../customer/proposals/components/ProposalStatusChip";
import { proposalStatusLabel } from "../../../customer/proposals/model/proposalDisplay";
import { t } from "../../../../shared/i18n";
import { groupByStatus } from "../../../../shared/marketplace/groupByStatus";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { useMyProviderDeals } from "../../deals/hooks/useMyProviderDeals";
import { useMyProviderExecutions } from "../../executions/hooks/useMyProviderExecutions";
import { providerExecutionStatusLabel } from "../../executions/model/providerExecutionDisplay";
import { useMyProviderProfile } from "../../profile/hooks/useMyProviderProfile";
import type { ProviderProfile } from "../../profile/api/providerProfileTypes";
import { useMyProviderProposals } from "../../proposals/hooks/useMyProviderProposals";
import { RequestCard } from "../../requests/components/RequestCard";
import { useProviderRequestInbox } from "../../requests/hooks/useProviderRequestInbox";

const PREVIEW = 4;

function profileChecks(profile: ProviderProfile) {
  return [
    { ok: Boolean(profile.name.trim()), label: t("provider.profile.name") },
    { ok: Boolean(profile.mobile.trim()), label: t("provider.profile.mobile") },
    { ok: Boolean(profile.description?.trim()), label: t("provider.profile.descriptionField") },
    {
      ok: profile.lat != null && profile.lng != null,
      label: t("provider.profile.coordinates"),
    },
  ];
}

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

  const nameSuffix = profile.data?.name ? `، ${profile.data.name}` : "";
  const proposalGroups = groupByStatus(proposals.data ?? [], (item) => item.status, [
    "Pending",
    "Accepted",
    "Rejected",
  ]);
  const inboxPreview = (requests.data ?? []).slice(0, PREVIEW);
  const dealPreview = (deals.data ?? []).slice(0, PREVIEW);
  const executionPreview = (executions.data ?? []).slice(0, PREVIEW);

  return (
    <>
      <PageHeader
        title={t("provider.dashboard.title")}
        description={t("provider.dashboard.intro")}
      />

      <AppCard>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          spacing={1.5}
          alignItems={{ xs: "stretch", sm: "center" }}
        >
          <Stack spacing={0.5}>
            <Typography variant="h6">
              {t("provider.dashboard.welcome", { name: nameSuffix })}
            </Typography>
            {profile.data ? (
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                <StatusChip label={profile.data.status} tone="info" />
              </Stack>
            ) : null}
          </Stack>
          <Button
            component={RouterLink}
            to="/provider/profile"
            variant="outlined"
            sx={{ minHeight: 44 }}
          >
            {t("nav.profile")}
          </Button>
        </Stack>
      </AppCard>

      <Typography variant="h6" sx={{ mt: 3, mb: 1.5 }}>
        {t("provider.dashboard.workspace")}
      </Typography>
      <ResponsiveCardGrid>
        <ActionCard
          title={t("provider.dashboard.inbox")}
          body={t("provider.dashboard.inboxHint")}
          to="/provider/requests"
        />
        <ActionCard
          title={t("provider.dashboard.proposals")}
          body={t("provider.dashboard.proposalsHint")}
          to="/provider/proposals"
        />
        <ActionCard
          title={t("provider.dashboard.deals")}
          body={t("provider.dashboard.dealsHint")}
          to="/provider/deals"
        />
        <ActionCard
          title={t("provider.dashboard.executions")}
          body={t("provider.dashboard.executionsHint")}
          to="/provider/executions"
        />
      </ResponsiveCardGrid>

      {profile.data ? (
        <Stack sx={{ mt: 3 }} spacing={1.5}>
          <Typography variant="h6">{t("provider.dashboard.profileCompleteness")}</Typography>
          <AppCard>
            <Stack spacing={1}>
              {profileChecks(profile.data).map((check) => (
                <Typography
                  key={check.label}
                  variant="body2"
                  color={check.ok ? "text.primary" : "text.secondary"}
                >
                  {check.ok
                    ? t("provider.dashboard.fieldPresent", { field: check.label })
                    : t("provider.dashboard.fieldMissing", { field: check.label })}
                </Typography>
              ))}
              <Typography variant="body2">
                {t("provider.profile.rating", { rating: profile.data.rating })}
              </Typography>
              <Typography variant="body2">
                {t("provider.profile.reviewCount", { count: profile.data.reviewCount })}
              </Typography>
              <Typography variant="body2">
                {t("provider.profile.completedJobs", { count: profile.data.completedJobCount })}
              </Typography>
            </Stack>
          </AppCard>
        </Stack>
      ) : null}

      {isPending ? <LoadingState label={t("provider.dashboard.loading")} /> : null}
      {isError ? (
        <Stack spacing={2} sx={{ mt: 2 }}>
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
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
          >
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}

      {!isPending && !isError ? (
        <Stack spacing={3} sx={{ mt: 3 }}>
          <DashboardSection
            title={t("provider.dashboard.incoming")}
            to="/provider/requests"
            empty={!inboxPreview.length}
            emptyTitle={t("provider.requests.emptyTitle")}
            emptyBody={t("provider.requests.emptyBody")}
          >
            <ResponsiveCardGrid>
              {inboxPreview.map((item) => (
                <RequestCard key={item.requestId} item={item} />
              ))}
            </ResponsiveCardGrid>
          </DashboardSection>

          <DashboardSection
            title={t("provider.dashboard.proposalStatus")}
            to="/provider/proposals"
            empty={proposalGroups.length === 0}
            emptyTitle={t("provider.proposals.emptyTitle")}
            emptyBody={t("provider.proposals.emptyBody")}
          >
            <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
              {proposalGroups.map((group) => (
                <Box key={group.status}>
                  <ProposalStatusChip status={group.status} />
                  <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 0.5 }}>
                    {proposalStatusLabel(group.status)} · {group.items.length}
                  </Typography>
                </Box>
              ))}
            </Stack>
          </DashboardSection>

          <DashboardSection
            title={t("provider.dashboard.activeDeals")}
            to="/provider/deals"
            empty={!dealPreview.length}
            emptyTitle={t("provider.deals.emptyTitle")}
            emptyBody={t("provider.deals.emptyBody")}
          >
            <Stack spacing={1}>
              {dealPreview.map((deal) => (
                <AppCard key={deal.id}>
                  <Stack
                    direction={{ xs: "column", sm: "row" }}
                    justifyContent="space-between"
                    spacing={1}
                  >
                    <Typography variant="subtitle1">
                      {t("provider.deals.dealId", { id: deal.id })}
                    </Typography>
                    <DealStatusChip status={deal.status} />
                  </Stack>
                </AppCard>
              ))}
            </Stack>
          </DashboardSection>

          <DashboardSection
            title={t("provider.dashboard.executionState")}
            to="/provider/executions"
            empty={!executionPreview.length}
            emptyTitle={t("provider.executions.emptyTitle")}
            emptyBody={t("provider.executions.emptyBody")}
          >
            <Stack spacing={1}>
              {executionPreview.map((execution) => (
                <AppCard key={execution.id}>
                  <Stack
                    direction={{ xs: "column", sm: "row" }}
                    justifyContent="space-between"
                    spacing={1}
                  >
                    <Typography variant="subtitle1">
                      {t("deal.execution.id", { id: execution.id })}
                    </Typography>
                    <Typography variant="body2">
                      {providerExecutionStatusLabel(execution.status)}
                    </Typography>
                  </Stack>
                </AppCard>
              ))}
            </Stack>
          </DashboardSection>
        </Stack>
      ) : null}
    </>
  );
}

function ActionCard({ title, body, to }: { title: string; body: string; to: string }) {
  return (
    <AppCard>
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Typography variant="subtitle1">{title}</Typography>
        <Typography variant="body2" color="text.secondary">
          {body}
        </Typography>
        <Button
          component={RouterLink}
          to={to}
          variant="contained"
          sx={{ mt: "auto", minHeight: 44, width: { xs: "100%", sm: "auto" }, alignSelf: { sm: "flex-start" } }}
        >
          {title}
        </Button>
      </Stack>
    </AppCard>
  );
}

function DashboardSection({
  title,
  to,
  empty,
  emptyTitle,
  emptyBody,
  children,
}: {
  title: string;
  to: string;
  empty: boolean;
  emptyTitle: string;
  emptyBody: string;
  children: ReactNode;
}) {
  return (
    <Stack spacing={1.5}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "stretch", sm: "center" }}
        spacing={1}
      >
        <Typography variant="h6">{title}</Typography>
        <Button component={RouterLink} to={to} variant="text" sx={{ alignSelf: { sm: "flex-end" } }}>
          {t("provider.dashboard.viewAll")}
        </Button>
      </Stack>
      {empty ? <EmptyState title={emptyTitle} body={emptyBody} /> : children}
    </Stack>
  );
}
