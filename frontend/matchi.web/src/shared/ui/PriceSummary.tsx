import { Divider, Stack, Typography } from "@mui/material";
import { getLocale, t } from "../i18n";

type PriceSummaryProps = {
  subtotal?: number;
  deliveryFee?: number;
  total: number;
};

function formatMoney(value: number): string {
  return new Intl.NumberFormat(getLocale(), {
    maximumFractionDigits: 0,
  }).format(value);
}

function Row({
  label,
  value,
  emphasize,
}: {
  label: string;
  value: string;
  emphasize?: boolean;
}) {
  return (
    <Stack direction="row" justifyContent="space-between" alignItems="baseline" spacing={2}>
      <Typography
        variant={emphasize ? "subtitle1" : "body2"}
        color={emphasize ? "text.primary" : "text.secondary"}
        fontWeight={emphasize ? 700 : 400}
      >
        {label}
      </Typography>
      <Typography
        variant={emphasize ? "h5" : "body1"}
        fontWeight={emphasize ? 800 : 600}
        sx={emphasize ? { color: "primary.main" } : undefined}
      >
        {value}
      </Typography>
    </Stack>
  );
}

export function PriceSummary({ subtotal, deliveryFee, total }: PriceSummaryProps) {
  return (
    <Stack spacing={1.25} aria-label={t("proposal.pricingSummary")}>
      {subtotal != null ? (
        <Row label={t("proposal.subtotal")} value={formatMoney(subtotal)} />
      ) : null}
      {deliveryFee != null ? (
        <Row label={t("proposal.deliveryFee")} value={formatMoney(deliveryFee)} />
      ) : null}
      <Divider />
      <Row label={t("proposal.total")} value={formatMoney(total)} emphasize />
    </Stack>
  );
}
