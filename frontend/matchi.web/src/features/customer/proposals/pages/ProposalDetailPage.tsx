import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { buildCustomerJourney } from "../../../../shared/marketplace/customerJourney";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { FormSplitLayout } from "../../../../shared/ui/FormSplitLayout";
import { JourneyTimeline } from "../../../../shared/ui/JourneyTimeline";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel } from "../../requests/api/requestTypes";
import { useRequest } from "../../requests/hooks/useRequest";
import type { ProposalDetail, ProposalItem } from "../api/proposalTypes";
import { AcceptProposalButton } from "../components/AcceptProposalButton";
import { ProposalStatusChip } from "../components/ProposalStatusChip";
import { useProposal } from "../hooks/useProposal";
import {
  formatDateTime,
  formatMoney,
  formatProposalSchedule,
  parsePositiveId,
  proposalItemsSubtotal,
  proposerPartyLabel,
} from "../model/proposalDisplay";

export function ProposalDetailPage() {
  const { id } = useParams();
  const proposalId = parsePositiveId(id);
  const { data, isPending, isError, error } = useProposal(proposalId);

  if (proposalId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("proposal.invalidLink") })}
      />
    );
  }

  return (
    <>
      <PageHeader title={t("proposal.detail.title")} />
      {isPending ? <LoadingState label={t("proposal.detail.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}
      {data ? <ProposalDetailBody proposal={data} /> : null}
    </>
  );
}

function ProposalDetailBody({ proposal }: { proposal: ProposalDetail }) {
  const requestQuery = useRequest(proposal.requestId);
  const items = [...proposal.items].sort(
    (a, b) => a.displayOrder - b.displayOrder || a.id - b.id,
  );
  const schedule = formatProposalSchedule(proposal);
  const accepted = proposal.status.toLowerCase() === "accepted";
  const journey = buildCustomerJourney({
    current: "proposal",
    requestId: proposal.requestId,
    requestExists: Boolean(requestQuery.data),
    proposalAccepted: accepted,
  });

  return (
    <Stack spacing={2}>
      <JourneyTimeline steps={journey} />
      <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
        <ProposalStatusChip status={proposal.status} />
      </Stack>

      <FormSplitLayout
        wide
        main={
          <Stack spacing={2}>
            <AppCard>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {t("proposal.detail.party")}
              </Typography>
              <Typography variant="body1">
                {proposerPartyLabel(proposal.proposerType, proposal.proposerId)}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                {t("proposal.compare.type")}: {proposal.proposerType}
              </Typography>
              <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 2 }}>
                {t("proposal.created", { date: formatDateTime(proposal.createDate) })}
              </Typography>
              {proposal.expireAt ? (
                <Typography variant="caption" color="text.secondary" display="block">
                  {t("proposal.expire", { date: formatDateTime(proposal.expireAt) })}
                </Typography>
              ) : null}
            </AppCard>

            <AppCard>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {t("proposal.detail.message")}
              </Typography>
              <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
                {proposal.message?.trim() ? proposal.message : t("proposal.noMessage")}
              </Typography>
              <Typography variant="subtitle2" sx={{ mt: 2 }}>
                {t("proposal.detail.schedule")}
              </Typography>
              <Typography variant="body2">
                {schedule ?? t("proposal.compare.noSchedule")}
              </Typography>
            </AppCard>

            <AppCard>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {t("proposal.detail.request")}
              </Typography>
              {requestQuery.isPending ? (
                <LoadingState label={t("request.detail.loading")} />
              ) : null}
              {requestQuery.isError ? <ErrorAlert error={requestQuery.error} /> : null}
              {requestQuery.data ? (
                <Stack spacing={1}>
                  <Typography variant="subtitle1">{requestQuery.data.title}</Typography>
                  <StatusChip label={requestKindLabel(requestQuery.data.requestType)} tone="info" />
                  <Button
                    component={RouterLink}
                    to={`/customer/requests/${proposal.requestId}`}
                    variant="text"
                    sx={{ alignSelf: "flex-start" }}
                  >
                    {t("proposal.detail.openRequest")}
                  </Button>
                </Stack>
              ) : null}
            </AppCard>

            <Typography variant="h6">{t("proposal.detail.items")}</Typography>
            {items.length === 0 ? (
              <EmptyState title={t("proposal.detail.noItems")} />
            ) : (
              <Stack spacing={1}>
                {items.map((item) => (
                  <ProposalItemCard key={item.id} item={item} />
                ))}
              </Stack>
            )}
          </Stack>
        }
        summary={
          <AppCard>
            <Stack spacing={2}>
              <Typography variant="h6">{t("proposal.pricingSummary")}</Typography>
              <PriceSummary
                subtotal={proposalItemsSubtotal(proposal.items)}
                deliveryFee={proposal.deliveryFee}
                total={proposal.totalPrice}
              />
              <AcceptProposalButton proposalId={proposal.id} status={proposal.status} fullWidth />
            </Stack>
          </AppCard>
        }
      />
    </Stack>
  );
}

function ProposalItemCard({ item }: { item: ProposalItem }) {
  const isService = item.itemType.toLowerCase() === "service";
  const label = isService
    ? t("proposal.detail.itemService", { id: item.serviceId ?? "—" })
    : t("proposal.detail.itemProduct", { id: item.productId ?? "—" });

  return (
    <AppCard>
      <Typography variant="subtitle1">{label}</Typography>
      {item.description ? (
        <Typography variant="body2" color="text.secondary">
          {item.description}
        </Typography>
      ) : null}
      <Typography variant="body2" color="text.secondary">
        {item.quantity} × {formatMoney(item.unitPrice)} = {formatMoney(item.totalPrice)}
      </Typography>
    </AppCard>
  );
}
