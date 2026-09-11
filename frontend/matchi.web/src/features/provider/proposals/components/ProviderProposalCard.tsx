import { Stack, Typography } from "@mui/material";
import {
  formatDateTime,
  formatMoney,
  proposalStatusLabel,
} from "../../../customer/proposals/model/proposalDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ProviderProposal } from "../api/providerProposalTypes";

type ProviderProposalCardProps = {
  proposal: ProviderProposal;
};

export function ProviderProposalCard({ proposal }: ProviderProposalCardProps) {
  return (
    <AppCard>
      <Stack spacing={1}>
        <StatusChip label={proposalStatusLabel(proposal.status)} />
        <Typography variant="subtitle1">{t("deal.proposalRef", { id: proposal.id })}</Typography>
        <Typography variant="body2">{t("deal.requestRef", { id: proposal.requestId })}</Typography>
        <Typography variant="body2">
          {t("proposal.price")}: {formatMoney(proposal.totalPrice)}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {proposal.dealId == null
            ? t("provider.proposals.noDeal")
            : `${t("proposal.viewDeal")} #${proposal.dealId}`}
        </Typography>
        <Typography variant="caption" color="text.secondary">
          {t("proposal.created", { date: formatDateTime(proposal.createDate) })}
        </Typography>
      </Stack>
    </AppCard>
  );
}
