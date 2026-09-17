import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import type { ProposalDetail, ProposalListItem } from "../api/proposalTypes";
import {
  formatDateTime,
  formatProposalSchedule,
  proposalItemsSubtotal,
  proposerPartyLabel,
} from "../model/proposalDisplay";
import { AcceptProposalButton } from "./AcceptProposalButton";
import { RejectProposalButton } from "./RejectProposalButton";
import { ProposalStatusChip } from "./ProposalStatusChip";

type ProposalCardProps = {
  proposal: ProposalListItem;
  detail?: ProposalDetail;
  detailLoading?: boolean;
  detailFailed?: boolean;
};

function itemLineLabel(item: ProposalDetail["items"][number]): string {
  const isService = item.itemType.toLowerCase() === "service";
  const name = isService
    ? t("proposal.detail.itemService", { id: item.serviceId ?? "—" })
    : t("proposal.detail.itemProduct", { id: item.productId ?? "—" });
  return `${name} · ${item.quantity}`;
}

export function ProposalCard({
  proposal,
  detail,
  detailLoading,
  detailFailed,
}: ProposalCardProps) {
  const schedule = detail
    ? formatProposalSchedule(detail)
    : null;
  const items = detail
    ? [...detail.items].sort((a, b) => a.displayOrder - b.displayOrder || a.id - b.id)
    : [];
  const subtotal = detail ? proposalItemsSubtotal(detail.items) : proposal.totalPrice - proposal.deliveryFee;
  const message = detail?.message?.trim();

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          alignItems={{ xs: "flex-start", sm: "center" }}
          spacing={1}
        >
          <Typography variant="h6">
            {proposerPartyLabel(proposal.proposerType, proposal.proposerId)}
          </Typography>
          <ProposalStatusChip status={proposal.status} />
        </Stack>
        <Typography variant="body2" color="text.secondary">
          {t("proposal.compare.type")}: {proposal.proposerType}
        </Typography>
        <Typography variant="subtitle2">{t("proposal.detail.items")}</Typography>
        {detailLoading ? (
          <Typography variant="body2" color="text.secondary">
            {t("proposal.compare.loadingItems")}
          </Typography>
        ) : null}
        {detailFailed ? (
          <Typography variant="body2" color="text.secondary">
            {t("proposal.compare.itemsUnavailable")}
          </Typography>
        ) : null}
        {detail && items.length === 0 ? (
          <Typography variant="body2" color="text.secondary">
            {t("proposal.detail.noItems")}
          </Typography>
        ) : null}
        {items.map((item) => (
          <Typography key={item.id} variant="body2">
            {itemLineLabel(item)}
          </Typography>
        ))}
        <PriceSummary
          subtotal={subtotal}
          deliveryFee={proposal.deliveryFee}
          total={proposal.totalPrice}
        />
        <Typography variant="body2">
          {t("proposal.detail.schedule")}: {schedule ?? t("proposal.compare.noSchedule")}
        </Typography>
        <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
          {detailLoading
            ? t("proposal.compare.loadingItems")
            : message
              ? message
              : t("proposal.noMessage")}
        </Typography>
        {proposal.expireAt ? (
          <Typography variant="caption" color="text.secondary">
            {t("proposal.expire", { date: formatDateTime(proposal.expireAt) })}
          </Typography>
        ) : null}
        <Typography variant="caption" color="text.secondary">
          {t("proposal.created", { date: formatDateTime(proposal.createDate) })}
        </Typography>
        <AcceptProposalButton proposalId={proposal.id} status={proposal.status} />
        <RejectProposalButton proposalId={proposal.id} status={proposal.status} />
        <Button
          component={RouterLink}
          to={`/customer/proposals/${proposal.id}`}
          variant="outlined"
          sx={{ minHeight: 44, alignSelf: { xs: "stretch", sm: "flex-start" } }}
        >
          {t("proposal.view")}
        </Button>
      </Stack>
    </AppCard>
  );
}
