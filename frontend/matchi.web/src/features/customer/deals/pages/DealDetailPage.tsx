import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel } from "../../requests/api/requestTypes";
import { useRequest } from "../../requests/hooks/useRequest";
import { useProposal } from "../../proposals/hooks/useProposal";
import {
  formatMoney,
  proposalStatusLabel,
  proposerPartyLabel,
} from "../../proposals/model/proposalDisplay";
import type { DealDetail } from "../api/dealTypes";
import { DealReviewSection } from "../../reviews/components/DealReviewSection";
import { DealExecutionSection } from "../components/DealExecutionSection";
import { DealSummary } from "../components/DealSummary";
import { useDeal } from "../hooks/useDeal";
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
      <PageHeader title={t("deal.detail.title")} />
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

  return (
    <Stack spacing={2}>
      <AppCard>
        <DealSummary deal={deal} />
      </AppCard>

      <AppCard>
        <Typography variant="subtitle2" color="text.secondary">
          {t("deal.detail.request")}
        </Typography>
        {requestQuery.isPending ? <LoadingState label={t("request.detail.loading")} /> : null}
        {requestQuery.isError ? <ErrorAlert error={requestQuery.error} /> : null}
        {requestQuery.data ? (
          <Stack spacing={1} sx={{ mt: 1 }}>
            <Typography variant="subtitle1">{requestQuery.data.title}</Typography>
            <StatusChip label={requestKindLabel(requestQuery.data.requestType)} />
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
        <Typography variant="subtitle2" color="text.secondary">
          {t("deal.detail.proposal")}
        </Typography>
        {proposalQuery.isPending ? <LoadingState label={t("proposal.detail.loading")} /> : null}
        {proposalQuery.isError ? <ErrorAlert error={proposalQuery.error} /> : null}
        {proposalQuery.data ? (
          <Stack spacing={1} sx={{ mt: 1 }}>
            <Typography variant="body2">
              {t("deal.detail.party")}:{" "}
              {proposerPartyLabel(
                proposalQuery.data.proposerType,
                proposalQuery.data.proposerId,
              )}
            </Typography>
            <StatusChip label={proposalStatusLabel(proposalQuery.data.status)} />
            <Typography variant="body2">
              {t("proposal.price")}: {formatMoney(proposalQuery.data.totalPrice)}
            </Typography>
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

      <DealExecutionSection dealId={deal.id} requestType={requestQuery.data?.requestType} />

      <DealReviewSection deal={deal} />
    </Stack>
  );
}
