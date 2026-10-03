import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { ItemActions } from "../../../../shared/ui/OverflowActions";
import { SectionHeader } from "../../../../shared/ui/SectionHeader";
import { RequestStatusChip } from "../../requests/components/RequestStatusChip";
import { Add } from "../../../../shared/ui/icons";
import { useMyDeals } from "../../deals/hooks/useMyDeals";
import { useMyRequests } from "../../requests/hooks/useMyRequests";
import { isRequestOpen } from "../../requests/model/requestPresentation";

export function CustomerDashboardPage() {
  const { user } = useAuth();
  const requestsQuery = useMyRequests();
  const dealsQuery = useMyDeals();
  const requests = requestsQuery.data ?? [];
  const deals = dealsQuery.data ?? [];
  const openRequests = requests.filter((item) => isRequestOpen(item.status));
  const activeDeals = deals.filter((item) => item.status.toLowerCase() === "active");
  const recent = requests.slice(0, 5);
  const isPending = requestsQuery.isPending || dealsQuery.isPending;
  const isError = requestsQuery.isError;
  const error = requestsQuery.error;

  return (
    <Stack spacing={{ xs: 2.5, md: 3 }}>
      <Stack spacing={0.75}>
        <Typography variant="h4" component="h1">
          {t("dashboard.welcome", { mobile: user?.mobile ? `، ${user.mobile}` : "" })}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t("dashboard.intro")}
        </Typography>
        <Button
          component={RouterLink}
          to="/customer/requests/create"
          variant="contained"
          size="large"
          startIcon={<Add />}
          sx={{ minHeight: 48, alignSelf: { xs: "stretch", sm: "flex-start" }, fontWeight: 700, mt: 0.5 }}
        >
          {t("request.create.new")}
        </Button>
      </Stack>

      {isPending ? <LoadingState label={t("dashboard.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}

      {!isPending && !isError && requests.length === 0 && activeDeals.length === 0 ? (
        <EmptyState title={t("dashboard.emptyTitle")} body={t("dashboard.emptyBody")} illustration="requests" />
      ) : null}

      {!isPending && !isError && (openRequests.length > 0 || activeDeals.length > 0) ? (
        <Stack spacing={1.25}>
          <SectionHeader compact title={t("dashboard.attention")} />
          <Box
            sx={{
              bgcolor: "background.paper",
              border: 1,
              borderColor: "divider",
              borderRadius: 2,
            }}
          >
            {openRequests.map((item, index) => (
              <Box
                key={`req-${item.id}`}
                sx={{
                  px: 1.5,
                  py: 1.25,
                  borderTop: index === 0 ? 0 : 1,
                  borderColor: "divider",
                }}
              >
                <Stack spacing={1}>
                  <Stack direction="row" spacing={1} alignItems="center" justifyContent="space-between">
                    <Box sx={{ minWidth: 0, flex: 1 }}>
                      <Typography variant="body2" fontWeight={600} noWrap>
                        {item.title}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {t("dashboard.attentionOpen")}
                      </Typography>
                    </Box>
                    <RequestStatusChip status={item.status} />
                  </Stack>
                  <ItemActions
                    primary={
                      <Button
                        component={RouterLink}
                        to={`/customer/requests/${item.id}`}
                        size="small"
                        variant="contained"
                        sx={{ minHeight: 40 }}
                      >
                        {t("request.card.view")}
                      </Button>
                    }
                    items={[
                      {
                        key: "matches",
                        label: t("dashboard.nextMatching"),
                        to: `/customer/requests/${item.id}/matches`,
                      },
                      {
                        key: "proposals",
                        label: t("dashboard.nextProposals"),
                        to: `/customer/requests/${item.id}/proposals`,
                      },
                    ]}
                  />
                </Stack>
              </Box>
            ))}
            {activeDeals.map((deal) => (
              <Box
                key={`deal-${deal.id}`}
                sx={{
                  px: 1.5,
                  py: 1.25,
                  borderTop: 1,
                  borderColor: "divider",
                }}
              >
                <Stack spacing={1}>
                  <Box>
                    <Typography variant="body2" fontWeight={600}>
                      {t("deal.requestRef", { id: deal.requestId })}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {t("dashboard.attentionDeal")}
                    </Typography>
                  </Box>
                  <ItemActions
                    primary={
                      <Button
                        component={RouterLink}
                        to={`/customer/deals/${deal.id}`}
                        size="small"
                        variant="contained"
                        sx={{ minHeight: 40 }}
                      >
                        {t("deal.view")}
                      </Button>
                    }
                  />
                </Stack>
              </Box>
            ))}
          </Box>
        </Stack>
      ) : null}

      {!isPending && !isError && recent.length > 0 ? (
        <Stack spacing={1.25}>
          <SectionHeader compact title={t("dashboard.recentActivity")} />
          <Box
            sx={{
              bgcolor: "background.paper",
              border: 1,
              borderColor: "divider",
              borderRadius: 2,
            }}
          >
            {recent.map((item, index) => (
              <Box
                key={item.id}
                sx={{
                  px: 1.5,
                  py: 1.25,
                  borderTop: index === 0 ? 0 : 1,
                  borderColor: "divider",
                }}
              >
                <Stack spacing={0.5}>
                  <Typography variant="body2" fontWeight={600} noWrap>
                    {item.title}
                  </Typography>
                  <ItemActions
                    primary={
                      <Button
                        component={RouterLink}
                        to={`/customer/requests/${item.id}`}
                        size="small"
                        variant="outlined"
                        sx={{ minHeight: 40 }}
                      >
                        {t("request.card.view")}
                      </Button>
                    }
                  />
                </Stack>
              </Box>
            ))}
          </Box>
        </Stack>
      ) : null}

      {!isPending && !isError && requests.length > 0 ? (
        <Typography variant="caption" color="text.secondary">
          {t("dashboard.summaryLine", { total: requests.length, open: openRequests.length })}
        </Typography>
      ) : null}
    </Stack>
  );
}
