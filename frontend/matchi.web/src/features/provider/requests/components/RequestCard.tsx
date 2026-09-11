import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { requestKindLabel } from "../../../customer/requests/api/requestTypes";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ProviderInboxLocation, ProviderRequestInboxItem } from "../api/providerRequestTypes";

type RequestCardProps = {
  item: ProviderRequestInboxItem;
};

function locationLines(location: ProviderInboxLocation): string[] {
  const lines: string[] = [];
  if (location.province?.trim()) {
    lines.push(t("provider.requests.province", { value: location.province.trim() }));
  }
  if (location.city?.trim()) {
    lines.push(t("provider.requests.city", { value: location.city.trim() }));
  }
  if (location.district?.trim()) {
    lines.push(t("provider.requests.district", { value: location.district.trim() }));
  }
  return lines;
}

export function RequestCard({ item }: RequestCardProps) {
  const location = item.location ? locationLines(item.location) : [];

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <StatusChip label={requestKindLabel(item.requestType)} />
          <StatusChip label={item.status} />
        </Stack>
        <Typography variant="subtitle1">
          {t("provider.requests.requestId", { id: item.requestId })}
        </Typography>
        {item.serviceSummary ? (
          <Typography variant="body2">
            {t("provider.requests.serviceSummary", { summary: item.serviceSummary })}
          </Typography>
        ) : null}
        {item.categorySummary ? (
          <Typography variant="body2" color="text.secondary">
            {t("provider.requests.categorySummary", { summary: item.categorySummary })}
          </Typography>
        ) : null}
        {location.length > 0 ? (
          <Stack spacing={0.25}>
            <Typography variant="body2" color="text.secondary">
              {t("provider.requests.location")}
            </Typography>
            {location.map((line) => (
              <Typography key={line} variant="body2" color="text.secondary">
                {line}
              </Typography>
            ))}
          </Stack>
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("provider.requests.noLocation")}
          </Typography>
        )}
        <Typography variant="caption" color="text.secondary">
          {t("request.detail.created", { date: formatDateTime(item.createdDate) })}
        </Typography>
        <Button
          component={RouterLink}
          to={`/provider/requests/${item.requestId}/proposal`}
          state={{ inboxItem: item }}
          variant="contained"
          size="small"
          sx={{ alignSelf: "flex-start" }}
        >
          {t("provider.requests.sendProposal")}
        </Button>
      </Stack>
    </AppCard>
  );
}
