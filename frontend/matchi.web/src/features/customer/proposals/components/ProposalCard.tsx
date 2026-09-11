import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import type { ProposalListItem } from "../api/proposalTypes";
import {
  formatDateTime,
  formatMoney,
  proposerPartyLabel,
} from "../model/proposalDisplay";
import { AcceptProposalButton } from "./AcceptProposalButton";
import { ProposalStatusChip } from "./ProposalStatusChip";

type ProposalCardProps = {
  proposal: ProposalListItem;
};

export function ProposalCard({ proposal }: ProposalCardProps) {
  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <ProposalStatusChip status={proposal.status} />
        </Stack>
        <Typography variant="subtitle1">
          {proposerPartyLabel(proposal.proposerType, proposal.proposerId)}
        </Typography>
        <Typography variant="body2">
          {t("proposal.price")}: {formatMoney(proposal.totalPrice)}
        </Typography>
        {proposal.deliveryFee > 0 ? (
          <Typography variant="body2" color="text.secondary">
            {t("proposal.deliveryFee")}: {formatMoney(proposal.deliveryFee)}
          </Typography>
        ) : null}
        <Typography variant="body2" color="text.secondary">
          {t("proposal.noMessage")}
        </Typography>
        <Typography variant="caption" color="text.secondary">
          {t("proposal.created", { date: formatDateTime(proposal.createDate) })}
        </Typography>
        <AcceptProposalButton proposalId={proposal.id} status={proposal.status} />
        <Button
          component={RouterLink}
          to={`/customer/proposals/${proposal.id}`}
          variant="outlined"
          sx={{ alignSelf: "flex-start" }}
        >
          {t("proposal.view")}
        </Button>
      </Stack>
    </AppCard>
  );
}
