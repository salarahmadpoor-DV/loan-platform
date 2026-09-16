import { Stack, Typography } from "@mui/material";
import { DealStatusChip } from "../../../customer/deals/components/DealStatusChip";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { PriceSummary } from "../../../../shared/ui/PriceSummary";
import type { ProviderDeal } from "../api/providerDealTypes";

type ProviderDealCardProps = {
  deal: ProviderDeal;
};

export function ProviderDealCard({ deal }: ProviderDealCardProps) {
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
          <Typography variant="h6">{t("provider.deals.dealId", { id: deal.id })}</Typography>
          <DealStatusChip status={deal.status} />
        </Stack>
        <Typography variant="body2">{t("deal.requestRef", { id: deal.requestId })}</Typography>
        <Typography variant="body2">{t("deal.proposalRef", { id: deal.proposalId })}</Typography>
        <PriceSummary total={deal.totalPrice} />
        <Typography variant="caption" color="text.secondary">
          {t("deal.accepted", { date: formatDateTime(deal.acceptedAt) })}
        </Typography>
      </Stack>
    </AppCard>
  );
}
