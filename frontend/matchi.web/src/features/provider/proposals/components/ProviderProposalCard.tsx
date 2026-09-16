import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { ProposalStatusChip } from "../../../customer/proposals/components/ProposalStatusChip";
import {
  formatDateTime,
  isPendingProposal,
} from "../../../customer/proposals/model/proposalDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import type { ProviderProposal } from "../api/providerProposalTypes";

type ProviderProposalCardProps = {
  proposal: ProviderProposal;
};

export function ProviderProposalCard({ proposal }: ProviderProposalCardProps) {
  const hasDeal = proposal.dealId != null;

  return (
    <AppCard>
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="flex-start"
          spacing={1}
          sx={{ flexWrap: "wrap" }}
          useFlexGap
        >
          <Typography variant="h6">{t("deal.proposalRef", { id: proposal.id })}</Typography>
          <ProposalStatusChip status={proposal.status} />
        </Stack>
        <Typography variant="body2">{t("deal.requestRef", { id: proposal.requestId })}</Typography>
        <PriceSummary total={proposal.totalPrice} />
        <Typography variant="caption" color="text.secondary">
          {t("proposal.created", { date: formatDateTime(proposal.createDate) })}
        </Typography>
        {hasDeal ? (
          <Typography variant="body2">
            {t("provider.proposals.linkedDeal", { id: proposal.dealId as number })}
          </Typography>
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("provider.proposals.noDeal")}
          </Typography>
        )}
        <Button
          component={RouterLink}
          to={hasDeal ? "/provider/deals" : "/provider/requests"}
          variant={hasDeal ? "contained" : isPendingProposal(proposal.status) ? "contained" : "outlined"}
          sx={{ mt: "auto", minHeight: 48, width: { xs: "100%", sm: "auto" }, alignSelf: { sm: "flex-start" } }}
        >
          {hasDeal
            ? t("provider.proposals.openDeals")
            : isPendingProposal(proposal.status)
              ? t("provider.proposals.backToInbox")
              : t("provider.proposals.openInbox")}
        </Button>
      </Stack>
    </AppCard>
  );
}
