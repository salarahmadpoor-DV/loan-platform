import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { t } from "../../../../shared/i18n";
import { REQUEST_KINDS } from "../../../../shared/types/marketplace";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { isRequestKind, requestKindLabel } from "../../requests/api/requestTypes";
import { useMyRequests } from "../../requests/hooks/useMyRequests";
import { isRequestOpen } from "../../requests/model/requestPresentation";

export function CustomerDashboardPage() {
  const { user } = useAuth();
  const { data, isPending, isError, error } = useMyRequests();
  const requests = data ?? [];
  const openRequests = requests.filter((item) => isRequestOpen(item.status));
  const openCount = openRequests.length;
  const focusRequest = openRequests[0];

  return (
    <>
      <PageHeader title={t("dashboard.title")} />

      <AppCard>
        <Typography variant="h6">
          {t("dashboard.welcome", { mobile: user?.mobile ? `، ${user.mobile}` : "" })}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          {t("dashboard.intro")}
        </Typography>
        <Stack direction="row" spacing={1} sx={{ mt: 2, flexWrap: "wrap" }} useFlexGap>
          {REQUEST_KINDS.map((kind) => (
            <StatusChip key={kind} label={requestKindLabel(kind)} />
          ))}
        </Stack>
        <Button
          component={RouterLink}
          to="/customer/requests/create"
          variant="contained"
          sx={{ mt: 2, width: { xs: "100%", sm: "auto" } }}
        >
          {t("request.list.create")}
        </Button>
      </AppCard>

      <Typography variant="subtitle1" sx={{ mt: 3, mb: 1 }}>
        {t("dashboard.summary")}
      </Typography>

      {isPending ? <LoadingState label={t("dashboard.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}

      {!isPending && !isError && requests.length === 0 ? (
        <EmptyState
          title={t("dashboard.emptyTitle")}
          body={t("dashboard.emptyBody")}
          action={
            <Button component={RouterLink} to="/customer/requests/create" variant="outlined">
              {t("request.list.create")}
            </Button>
          }
        />
      ) : null}

      {requests.length > 0 ? (
        <Box
          sx={{
            display: "grid",
            gap: 2,
            gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr", lg: "1fr 1fr 1fr" },
          }}
        >
          <SummaryCard label={t("dashboard.allRequests")} value={requests.length} />
          <SummaryCard label={t("dashboard.open")} value={openCount} />
          {REQUEST_KINDS.map((kind) => (
            <SummaryCard
              key={kind}
              label={requestKindLabel(kind)}
              value={
                requests.filter((item) => isRequestKind(item.requestType) && item.requestType === kind)
                  .length
              }
            />
          ))}
        </Box>
      ) : null}

      <Typography variant="subtitle1" sx={{ mt: 3, mb: 1 }}>
        {t("dashboard.next")}
      </Typography>
      <AppCard>
        <Typography variant="subtitle1">{t("dashboard.nextTitle")}</Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {t("dashboard.nextBody")}
        </Typography>
        <Stack direction={{ xs: "column", sm: "row" }} spacing={1} sx={{ mt: 2, flexWrap: "wrap" }} useFlexGap>
          {focusRequest ? (
            <>
              <Button
                component={RouterLink}
                to={`/customer/requests/${focusRequest.id}/matches`}
                variant="contained"
                sx={{ minHeight: 48 }}
              >
                {t("dashboard.nextMatching")}
              </Button>
              <Button
                component={RouterLink}
                to={`/customer/requests/${focusRequest.id}/proposals`}
                variant="outlined"
                sx={{ minHeight: 48 }}
              >
                {t("dashboard.nextProposals")}
              </Button>
            </>
          ) : (
            <Button
              component={RouterLink}
              to="/customer/requests/create"
              variant="contained"
              sx={{ minHeight: 48 }}
            >
              {t("request.list.create")}
            </Button>
          )}
          <Button
            component={RouterLink}
            to="/customer/deals"
            variant="outlined"
            sx={{ minHeight: 48 }}
          >
            {t("dashboard.nextDeals")}
          </Button>
        </Stack>
      </AppCard>
    </>
  );
}

function SummaryCard({ label, value }: { label: string; value: number }) {
  return (
    <AppCard>
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="h5" sx={{ mt: 0.5 }}>
        {value}
      </Typography>
    </AppCard>
  );
}
