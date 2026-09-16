import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { buildCustomerJourney } from "../../../../shared/marketplace/customerJourney";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { FormSplitLayout } from "../../../../shared/ui/FormSplitLayout";
import { JourneyTimeline } from "../../../../shared/ui/JourneyTimeline";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel } from "../../requests/api/requestTypes";
import { useRequest } from "../../requests/hooks/useRequest";
import { useProposal } from "../../proposals/hooks/useProposal";
import {
  proposalStatusLabel,
  proposerPartyLabel,
} from "../../proposals/model/proposalDisplay";
import type { DealDetail } from "../api/dealTypes";
import { DealReviewSection } from "../../reviews/components/DealReviewSection";
import { DealExecutionSection } from "../components/DealExecutionSection";
import { DealSummary } from "../components/DealSummary";
import { useDeal } from "../hooks/useDeal";
import { useDealExecutions } from "../hooks/useDealExecutions";
import { parsePositiveId } from "../model/dealDisplay";

export function DealDetailPage() {
  const { id } = useParams();
  const dealId = parsePositiveId(id);
  const { data, isPending, isError, error, refetch, isFetching } = useDeal(dealId);

  if (dealId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("deal.invalidLink") })}
      />
    );
  }

  return (
    <>
      <PageHeader title={t("deal.detail.title")} description={t("deal.detail.lifecycle")} />
      {isPending ? <LoadingState label={t("deal.detail.loading")} /> : null}
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
            {t("deal.detail.retry")}
          </Button>
        </Stack>
      ) : null}
      {data ? <DealDetailBody deal={data} /> : null}
    </>
  );
}

function DealDetailBody({ deal }: { deal: DealDetail }) {
  const requestQuery = useRequest(deal.requestId);
  const proposalQuery = useProposal(deal.proposalId);
  const executionsQuery = useDealExecutions(deal.id);
  const executions = executionsQuery.data ?? [];
  const requestType = requestQuery.data?.requestType;
  const executionCompleted = executions.some(
    (item) => item.status.toLowerCase() === "completed",
  );
  const executionInProgress =
    requestType !== "Product" &&
    executionsQuery.isSuccess &&
    executions.length > 0 &&
    !executionCompleted;
  const current =
    requestType === "Product" || executionCompleted
      ? "review"
      : executionInProgress
        ? "execution"
        : "deal";

  const journey = buildCustomerJourney({
    current,
    requestId: deal.requestId,
    dealId: deal.id,
    requestExists: Boolean(requestQuery.data),
    proposalAccepted: proposalQuery.data?.status.toLowerCase() === "accepted",
    dealExists: true,
    requestType,
    executionsLoaded: executionsQuery.isSuccess,
    hasExecution: executions.length > 0,
    executionCompleted,
  });

  return (
    <Stack spacing={2}>
      <Typography variant="h6">{t("deal.journey.title")}</Typography>
      <JourneyTimeline steps={journey} />

      <FormSplitLayout
        wide
        main={
          <Stack spacing={2}>
            <AppCard>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {t("deal.detail.request")}
              </Typography>
              {requestQuery.isPending ? <LoadingState label={t("request.detail.loading")} /> : null}
              {requestQuery.isError ? <ErrorAlert error={requestQuery.error} /> : null}
              {requestQuery.data ? (
                <Stack spacing={1}>
                  <Typography variant="subtitle1">{requestQuery.data.title}</Typography>
                  <StatusChip label={requestKindLabel(requestQuery.data.requestType)} tone="info" />
                  <Button
                    component={RouterLink}
                    to={`/customer/requests/${deal.requestId}`}
                    variant="text"
                    sx={{ alignSelf: "flex-start" }}
                  >
                    {t("deal.detail.openRequest")}
                  </Button>
                </Stack>
              ) : null}
            </AppCard>

            <AppCard>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {t("deal.detail.proposal")}
              </Typography>
              {proposalQuery.isPending ? (
                <LoadingState label={t("proposal.detail.loading")} />
              ) : null}
              {proposalQuery.isError ? <ErrorAlert error={proposalQuery.error} /> : null}
              {proposalQuery.data ? (
                <Stack spacing={1}>
                  <Typography variant="body2">
                    {t("deal.detail.party")}:{" "}
                    {proposerPartyLabel(
                      proposalQuery.data.proposerType,
                      proposalQuery.data.proposerId,
                    )}
                  </Typography>
                  <StatusChip
                    label={proposalStatusLabel(proposalQuery.data.status)}
                    tone={
                      proposalQuery.data.status.toLowerCase() === "accepted" ? "success" : "neutral"
                    }
                  />
                  <Button
                    component={RouterLink}
                    to={`/customer/proposals/${deal.proposalId}`}
                    variant="text"
                    sx={{ alignSelf: "flex-start" }}
                  >
                    {t("deal.detail.openProposal")}
                  </Button>
                </Stack>
              ) : null}
            </AppCard>

            <DealExecutionSection dealId={deal.id} requestType={requestType} />

            <DealReviewSection deal={deal} />
          </Stack>
        }
        summary={
          <AppCard>
            <Stack spacing={2}>
              <DealSummary deal={deal} />
              <PriceSummary
                subtotal={
                  proposalQuery.data
                    ? proposalQuery.data.items.reduce((sum, item) => sum + item.totalPrice, 0)
                    : undefined
                }
                deliveryFee={proposalQuery.data?.deliveryFee}
                total={deal.totalPrice}
              />
            </Stack>
          </AppCard>
        }
      />
    </Stack>
  );
}
