import { Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel, type RequestDto } from "../api/requestTypes";
import {
  formatRequestDate,
  requestDescriptionSnippet,
  requestLocationSummary,
} from "../model/requestPresentation";
import { RequestStatusChip } from "./RequestStatusChip";

type RequestContextCardProps = {
  request: RequestDto;
};

export function RequestContextCard({ request }: RequestContextCardProps) {
  const place = requestLocationSummary(request.location);
  const snippet = requestDescriptionSnippet(request.description, 180);

  return (
    <AppCard>
      <Stack spacing={1}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <StatusChip label={requestKindLabel(request.requestType)} tone="info" />
          <RequestStatusChip status={request.status} />
          <Typography variant="caption" color="text.secondary" sx={{ alignSelf: "center" }}>
            {t("request.card.id", { id: request.id })}
          </Typography>
        </Stack>
        <Typography variant="subtitle1" fontWeight={700}>
          {request.title}
        </Typography>
        {snippet ? (
          <Typography variant="body2" color="text.secondary">
            {snippet}
          </Typography>
        ) : null}
        {place ? (
          <Typography variant="body2" color="text.secondary">
            {place}
          </Typography>
        ) : null}
        <Typography variant="caption" color="text.secondary">
          {t("request.card.created", { date: formatRequestDate(request.createDate) })}
          {" · "}
          {t("request.card.counts", {
            services: request.services.length,
            products: request.products.length,
          })}
        </Typography>
      </Stack>
    </AppCard>
  );
}
