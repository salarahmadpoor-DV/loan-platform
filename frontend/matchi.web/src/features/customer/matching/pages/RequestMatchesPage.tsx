import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { buildCustomerJourney } from "../../../../shared/marketplace/customerJourney";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { JourneyTimeline } from "../../../../shared/ui/JourneyTimeline";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { RequestContextCard } from "../../requests/components/RequestContextCard";
import { useRequest } from "../../requests/hooks/useRequest";
import { isRequestCancelled, isRequestOpen } from "../../requests/model/requestPresentation";
import { MatchCard } from "../components/MatchCard";
import { MatchCardSkeletonGrid } from "../components/MatchCardSkeletonGrid";
import { MatchReasonList } from "../components/MatchReasonList";
import { useRequestMatches } from "../hooks/useRequestMatches";

function parseRequestId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}

export function RequestMatchesPage() {
  const { id } = useParams();
  const requestId = parseRequestId(id);
  const requestQuery = useRequest(requestId);
  const cancelled = requestQuery.data ? isRequestCancelled(requestQuery.data.status) : false;
  const matchesQuery = useRequestMatches(requestId, requestQuery.isSuccess && !cancelled);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("matching.invalidLink") })}
      />
    );
  }

  const backToRequest = (
    <Button
      component={RouterLink}
      to={`/customer/requests/${requestId}`}
      variant="outlined"
      sx={{ minHeight: 48 }}
    >
      {t("matching.backToRequest")}
    </Button>
  );

  const requestOpen = requestQuery.data ? isRequestOpen(requestQuery.data.status) : false;

  return (
    <Box sx={{ maxWidth: 960, mx: "auto" }}>
      <PageHeader
        title={t("matching.pageTitle")}
        description={t("matching.pageDescription")}
        action={backToRequest}
      />
      <JourneyTimeline
        steps={buildCustomerJourney({
          current: "matching",
          requestId,
          requestExists: Boolean(requestQuery.data),
          matchesLoaded: Boolean(matchesQuery.data) && !matchesQuery.isError,
          hasMatches: (matchesQuery.data?.length ?? 0) > 0,
        })}
      />

      {requestQuery.isPending ? <LoadingState label={t("request.detail.loading")} /> : null}
      {requestQuery.isError ? (
        <Stack spacing={2} sx={{ mb: 2 }}>
          <ErrorAlert error={requestQuery.error} />
          <Button
            component={RouterLink}
            to="/customer/requests"
            variant="outlined"
            sx={{ alignSelf: "flex-start", minHeight: 48 }}
          >
            {t("request.detail.backToList")}
          </Button>
        </Stack>
      ) : null}
      {requestQuery.data ? (
        <Box sx={{ mb: 3 }}>
          <RequestContextCard request={requestQuery.data} />
        </Box>
      ) : null}

      {requestOpen ? (
        <Button
          component={RouterLink}
          to={`/customer/requests/${requestId}/proposals`}
          variant="text"
          sx={{ mb: 2, minHeight: 48 }}
        >
          {t("matching.viewProposals")}
        </Button>
      ) : null}

      {cancelled ? (
        <EmptyState title={t("matching.cancelledTitle")} body={t("matching.cancelledBody")} />
      ) : null}

      {!cancelled && matchesQuery.isPending ? (
        <Stack spacing={2} aria-busy="true" aria-live="polite">
          <LoadingState label={t("matching.loading")} />
          <MatchCardSkeletonGrid />
        </Stack>
      ) : null}

      {!cancelled && matchesQuery.isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={matchesQuery.error} />
          <Button
            variant="outlined"
            onClick={() => {
              void matchesQuery.refetch();
            }}
            disabled={matchesQuery.isFetching}
            sx={{ alignSelf: "flex-start", minHeight: 48 }}
          >
            {t("matching.retry")}
          </Button>
        </Stack>
      ) : null}

      {!cancelled && !matchesQuery.isPending && !matchesQuery.isError && matchesQuery.data?.length === 0 ? (
        <EmptyState
          title={t("matching.emptyTitle")}
          body={t("matching.emptyBody")}
          action={backToRequest}
        />
      ) : null}

      {!cancelled && matchesQuery.data && matchesQuery.data.length > 0 ? (
        <Stack spacing={2}>
          <Stack
            direction={{ xs: "column", sm: "row" }}
            spacing={1}
            alignItems={{ sm: "baseline" }}
            justifyContent="space-between"
          >
            <Typography variant="h3" component="h2">
              {t("matching.resultsTitle")}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t("matching.resultCount", { count: matchesQuery.data.length })}
            </Typography>
          </Stack>
          <ResponsiveCardGrid>
            {matchesQuery.data.map((match) => (
              <MatchCard key={`${match.candidateType}-${match.candidateId}`} match={match} />
            ))}
          </ResponsiveCardGrid>
          <MatchReasonList showAll />
        </Stack>
      ) : null}
    </Box>
  );
}
