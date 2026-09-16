import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { buildCustomerJourney } from "../../../../shared/marketplace/customerJourney";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { JourneyTimeline } from "../../../../shared/ui/JourneyTimeline";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ProposalCompareGrid } from "../components/ProposalCompareGrid";
import { useProposalDetails } from "../hooks/useProposalDetails";
import { useRequestProposals } from "../hooks/useRequestProposals";
import { parsePositiveId } from "../model/proposalDisplay";

export function RequestProposalsPage() {
  const { requestId: rawId } = useParams();
  const requestId = parsePositiveId(rawId);
  const { data, isPending, isError, error, refetch, isFetching } =
    useRequestProposals(requestId);
  const details = useProposalDetails(data?.map((item) => item.id) ?? []);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("proposal.invalidLink") })}
      />
    );
  }

  const journey = buildCustomerJourney({
    current: "proposal",
    requestId,
    requestExists: true,
    proposalsLoaded: Boolean(data) && !isError,
    hasProposals: (data?.length ?? 0) > 0,
  });

  return (
    <>
      <PageHeader
        title={t("proposal.list.title")}
        description={t("proposal.list.description")}
      />
      <JourneyTimeline steps={journey} />
      <Button
        component={RouterLink}
        to={`/customer/requests/${requestId}/matches`}
        variant="text"
        sx={{ mb: 2 }}
      >
        {t("proposal.backToMatching")}
      </Button>
      {isPending ? <LoadingState label={t("proposal.list.loading")} /> : null}
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
            {t("proposal.list.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("proposal.list.emptyTitle")}
          body={t("proposal.list.emptyBody")}
        />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          <Typography variant="h6">{t("proposal.compare.title")}</Typography>
          <ProposalCompareGrid
            proposals={data}
            details={details.map((query) => query.data)}
            loadingFlags={details.map((query) => query.isPending)}
            errorFlags={details.map((query) => query.isError)}
          />
        </Stack>
      ) : null}
    </>
  );
}
