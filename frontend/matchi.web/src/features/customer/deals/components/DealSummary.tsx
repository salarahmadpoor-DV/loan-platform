import { Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { formatDateTime, formatMoney } from "../../proposals/model/proposalDisplay";
import type { DealDetail, DealSummary } from "../api/dealTypes";
import { DealStatusChip } from "./DealStatusChip";

type DealSummaryProps = {
  deal: DealSummary | DealDetail;
};

export function DealSummary({ deal }: DealSummaryProps) {
  const createdAt = "createDate" in deal ? deal.createDate : deal.acceptedAt;

  return (
    <Stack spacing={0.5}>
      <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
        <DealStatusChip status={deal.status} />
      </Stack>
      <Typography variant="body2">{t("deal.requestRef", { id: deal.requestId })}</Typography>
      <Typography variant="body2">{t("deal.proposalRef", { id: deal.proposalId })}</Typography>
      <Typography variant="body2">
        {t("deal.price")}: {formatMoney(deal.totalPrice)}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        {t("deal.created", { date: formatDateTime(createdAt) })}
      </Typography>
      {"createDate" in deal ? (
        <Typography variant="caption" color="text.secondary">
          {t("deal.accepted", { date: formatDateTime(deal.acceptedAt) })}
        </Typography>
      ) : null}
    </Stack>
  );
}
