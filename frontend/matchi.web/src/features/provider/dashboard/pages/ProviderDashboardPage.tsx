import { Box, Button, Divider, List, ListItemButton, ListItemText, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { formatDateTime, isPendingProposal } from "../../../customer/proposals/model/proposalDisplay";
import { RequestStatusChip } from "../../../customer/requests/components/RequestStatusChip";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { ForwardIcon, NavIcon } from "../../../../shared/ui/icons";
import { rtlSafeFlexRow } from "../../../../shared/ui/noflipFlex";
import { useMyProviderDeals } from "../../deals/hooks/useMyProviderDeals";
import { useMyProviderExecutions } from "../../executions/hooks/useMyProviderExecutions";
import { useMyProviderInvitations } from "../../invitations/hooks/useMyProviderInvitations";
import { useMyProviderProfile } from "../../profile/hooks/useMyProviderProfile";
import { useMyProviderProposals } from "../../proposals/hooks/useMyProviderProposals";
import { useProviderRequestInbox } from "../../requests/hooks/useProviderRequestInbox";
import {
  buildProviderActions,
  buildRecentActivity,
  isCompletedExecution,
  isInProgressExecution,
  requestRowMeta,
  requestRowTitle,
} from "../model/dashboardPresentation";

const RECENT = 5;

export function ProviderDashboardPage() {
  const profile = useMyProviderProfile();
  const requests = useProviderRequestInbox();
  const proposals = useMyProviderProposals();
  const deals = useMyProviderDeals();
  const executions = useMyProviderExecutions();
  const invitations = useMyProviderInvitations();

  const isPending =
    requests.isPending || proposals.isPending || deals.isPending || executions.isPending;
  const firstError = requests.error ?? proposals.error ?? deals.error ?? executions.error;
  const isError = requests.isError || proposals.isError || deals.isError || executions.isError;

  const inbox = requests.data ?? [];
  const pendingProposals = (proposals.data ?? []).filter((item) => isPendingProposal(item.status));
  const attentionExecutions = (executions.data ?? []).filter((item) => isInProgressExecution(item.status));
  const completedCount = (executions.data ?? []).filter((item) => isCompletedExecution(item.status)).length;
  const displayName = profile.data?.name?.trim() ?? "";

  const actions = buildProviderActions({
    inbox,
    pendingProposals,
    invitations: invitations.data ?? [],
    attentionExecutions,
    labels: {
      requestTitle: t("provider.dashboard.actionRequestTitle"),
      requestBody: (count) => t("provider.dashboard.actionRequestBody", { count }),
      proposalTitle: t("provider.dashboard.actionProposalTitle"),
      proposalBody: (count) => t("provider.dashboard.actionProposalBody", { count }),
      inviteTitle: t("provider.dashboard.actionInviteTitle"),
      inviteBody: (count) => t("provider.dashboard.actionInviteBody", { count }),
      executionTitle: t("provider.dashboard.actionExecutionTitle"),
      executionBody: (count) => t("provider.dashboard.actionExecutionBody", { count }),
    },
  });

  const recentRequests = (requests.data ?? []).slice(0, RECENT);
  const activity = buildRecentActivity({
    proposals: proposals.data ?? [],
    deals: deals.data ?? [],
    executions: executions.data ?? [],
    labels: {
      proposal: (id) => t("provider.dashboard.activityProposal", { id }),
      deal: (id) => t("provider.dashboard.activityDeal", { id }),
      execution: (id) => t("provider.dashboard.activityExecution", { id }),
    },
  });

  return (
    <Stack spacing={{ xs: 2.5, md: 3 }}>
      <Stack spacing={0.75}>
        <Typography variant="h4" component="h1">
          {displayName
            ? t("provider.dashboard.hello", { name: displayName })
            : t("provider.dashboard.helloGeneric")}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t("provider.dashboard.readyHint")}
        </Typography>
      </Stack>

      {isPending ? <LoadingState label={t("provider.dashboard.loading")} /> : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={firstError} />
          <Button
            variant="outlined"
            onClick={() => {
              void profile.refetch();
              void requests.refetch();
              void proposals.refetch();
              void deals.refetch();
              void executions.refetch();
              void invitations.refetch();
            }}
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
          >
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}

      {!isPending && !isError ? (
        <Box
          sx={{
            display: "grid",
            gap: { xs: 2.5, md: 3 },
            gridTemplateColumns: { xs: "minmax(0, 1fr)", md: "minmax(0, 1.6fr) minmax(0, 0.9fr)" },
            alignItems: "start",
          }}
        >
          <Stack spacing={{ xs: 2.5, md: 3 }}>
            <Stack spacing={1.25}>
              <Typography variant="subtitle1" component="h2">
                {t("provider.dashboard.actionRequired")}
              </Typography>
              {actions.length === 0 ? (
                <Stack spacing={1.5} alignItems="flex-start">
                  <Typography variant="body2" color="text.secondary">
                    {t("provider.dashboard.noAction")}
                  </Typography>
                  <Button component={RouterLink} to="/provider/requests" variant="contained">
                    {t("provider.dashboard.viewInbox")}
                  </Button>
                </Stack>
              ) : (
                <List
                  disablePadding
                  sx={{
                    bgcolor: "rgba(37, 99, 235, 0.05)",
                    border: 1,
                    borderColor: "rgba(37, 99, 235, 0.16)",
                    borderRadius: 1,
                  }}
                >
                  {actions.map((row, index) => (
                    <Box key={row.id}>
                      {index > 0 ? <Divider /> : null}
                      <ListItemButton
                        component={RouterLink}
                        to={row.to}
                        style={{ ...rtlSafeFlexRow, alignItems: "flex-start", gap: 12 }}
                        sx={{ mx: 0, py: 1.25 }}
                      >
                        <NavIcon name={row.icon} sx={{ mt: 0.25, color: "primary.main" }} aria-hidden />
                        <ListItemText
                          primary={row.title}
                          secondary={row.body}
                          primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
                          secondaryTypographyProps={{ variant: "caption" }}
                        />
                        <ForwardIcon sx={{ color: "text.secondary", mt: 0.5 }} aria-hidden />
                      </ListItemButton>
                    </Box>
                  ))}
                </List>
              )}
            </Stack>

            <Stack spacing={1.25}>
              <Typography variant="subtitle1" component="h2">
                {t("provider.dashboard.recentRequests")}
              </Typography>
              {recentRequests.length === 0 ? (
                <EmptyState
                  title={t("provider.requests.emptyTitle")}
                  body={t("provider.requests.emptyBody")}
                  illustration="requests"
                />
              ) : (
                <List disablePadding sx={{ bgcolor: "background.paper", border: 1, borderColor: "divider", borderRadius: 1 }}>
                  {recentRequests.map((item, index) => (
                    <Box key={item.requestId}>
                      {index > 0 ? <Divider /> : null}
                      <ListItemButton
                        component={RouterLink}
                        to={`/provider/requests/${item.requestId}`}
                        sx={{ mx: 0, marginInline: 0, borderRadius: 0, py: 1.25, alignItems: "flex-start", gap: 1 }}
                      >
                        <ListItemText
                          primary={requestRowTitle(item)}
                          secondary={requestRowMeta(item) || formatDateTime(item.createdDate)}
                          primaryTypographyProps={{ variant: "body2", fontWeight: 600 }}
                          secondaryTypographyProps={{ variant: "caption" }}
                        />
                        <RequestStatusChip status={item.status} />
                        <ForwardIcon sx={{ color: "text.secondary", mt: 0.35 }} aria-hidden />
                      </ListItemButton>
                    </Box>
                  ))}
                </List>
              )}
            </Stack>
          </Stack>

          <Stack spacing={{ xs: 2.5, md: 3 }}>
            <Box
              sx={{
                display: "grid",
                gridTemplateColumns: "repeat(3, minmax(0, 1fr))",
                columnGap: 1.5,
                py: { xs: 0.5, md: 1 },
              }}
            >
              <QuietStat value={inbox.length} label={t("provider.dashboard.statsNew")} color="primary.main" />
              <QuietStat
                value={attentionExecutions.length}
                label={t("provider.dashboard.statsActive")}
                color="secondary.main"
              />
              <QuietStat value={completedCount} label={t("provider.dashboard.statsDone")} color="success.main" />
            </Box>

            <Stack spacing={1.25}>
              <Typography variant="subtitle1" component="h2">
                {t("provider.dashboard.recentActivity")}
              </Typography>
              {activity.length === 0 ? (
                <Typography variant="body2" color="text.secondary">
                  {t("provider.dashboard.emptyBody")}
                </Typography>
              ) : (
                <List disablePadding>
                  {activity.map((row) => (
                    <ListItemButton
                      key={row.id}
                      component={RouterLink}
                      to={row.to}
                      sx={{ mx: 0, marginInline: 0, px: 0, borderRadius: 1, minHeight: 44 }}
                    >
                      <ListItemText
                        primary={row.title}
                        secondary={formatDateTime(row.when)}
                        primaryTypographyProps={{ variant: "body2" }}
                        secondaryTypographyProps={{ variant: "caption" }}
                      />
                    </ListItemButton>
                  ))}
                </List>
              )}
            </Stack>
          </Stack>
        </Box>
      ) : null}
    </Stack>
  );
}

function QuietStat({
  value,
  label,
  color,
}: {
  value: number;
  label: string;
  color: "primary.main" | "secondary.main" | "success.main";
}) {
  return (
    <Box sx={{ minWidth: 0, textAlign: "center" }}>
      <Typography variant="h4" component="p" sx={{ fontWeight: 700, lineHeight: 1.2, color }}>
        {value}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
    </Box>
  );
}
