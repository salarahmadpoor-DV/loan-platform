import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel, type RequestDto } from "../api/requestTypes";
import { RequestStatusChip } from "./RequestStatusChip";

type RequestCardProps = {
  request: RequestDto;
};

function locationSummary(request: RequestDto): string | null {
  const location = request.location;
  if (!location) {
    return null;
  }
  const parts = [location.city, location.district, location.province].filter(
    (part): part is string => Boolean(part),
  );
  return parts.length > 0 ? parts.join(" · ") : null;
}

export function RequestCard({ request }: RequestCardProps) {
  const place = locationSummary(request);

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <StatusChip label={requestKindLabel(request.requestType)} />
          <RequestStatusChip status={request.status} />
        </Stack>
        <Typography variant="subtitle1">{request.title}</Typography>
        {place ? (
          <Typography variant="body2" color="text.secondary">
            {place}
          </Typography>
        ) : null}
        <Typography variant="caption" color="text.secondary">
          {t("request.card.counts", {
            services: request.services.length,
            products: request.products.length,
          })}
        </Typography>
        <Button
          component={RouterLink}
          to={`/customer/requests/${request.id}`}
          variant="outlined"
          size="small"
          sx={{ alignSelf: "flex-start" }}
        >
          {t("request.card.view")}
        </Button>
      </Stack>
    </AppCard>
  );
}
