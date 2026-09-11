import { Stack, Typography } from "@mui/material";
import { formatDateTime, formatMoney } from "../../../customer/proposals/model/proposalDisplay";
import { dealStatusLabel } from "../../../customer/deals/model/dealDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ProviderDeal } from "../api/providerDealTypes";

type ProviderDealCardProps = {
  deal: ProviderDeal;
};

export function ProviderDealCard({ deal }: ProviderDealCardProps) {
  return (
    <AppCard>
      <Stack spacing={1}>
        <StatusChip label={dealStatusLabel(deal.status)} />
        <Typography variant="subtitle1">#{deal.id}</Typography>
        <Typography variant="body2">{t("deal.requestRef", { id: deal.requestId })}</Typography>
        <Typography variant="body2">{t("deal.proposalRef", { id: deal.proposalId })}</Typography>
        <Typography variant="body2">
          {t("deal.price")}: {formatMoney(deal.totalPrice)}
        </Typography>
        <Typography variant="caption" color="text.secondary">
          {t("deal.accepted", { date: formatDateTime(deal.acceptedAt) })}
        </Typography>
      </Stack>
    </AppCard>
  );
}
