import { Button, Stack } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { MatchCard } from "../components/MatchCard";
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
  const { data, isPending, isError, error, refetch, isFetching } = useRequestMatches(requestId);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("matching.invalidLink") })}
      />
    );
  }

  return (
    <>
      <PageHeader
        title={t("matching.pageTitle")}
        description={t("matching.pageDescription")}
      />
      {requestId != null ? (
        <Button
          component={RouterLink}
          to={`/customer/requests/${requestId}/proposals`}
          variant="outlined"
          sx={{ mb: 2 }}
        >
          {t("matching.viewProposals")}
        </Button>
      ) : null}
      {isPending ? <LoadingState label={t("matching.loading")} /> : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ alignSelf: "flex-start" }}
          >
            {t("matching.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState title={t("matching.emptyTitle")} body={t("matching.emptyBody")} />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((match) => (
            <MatchCard
              key={`${match.candidateType}-${match.candidateId}`}
              match={match}
            />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
