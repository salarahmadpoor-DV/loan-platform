import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { requestKindLabel } from "../../../customer/requests/api/requestTypes";
import { RequestStatusChip } from "../../../customer/requests/components/RequestStatusChip";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ProviderRequestInboxItem } from "../api/providerRequestTypes";
import { inboxHeading, inboxLocationLines } from "../model/inboxDisplay";

type RequestCardProps = {
  item: ProviderRequestInboxItem;
};

export function RequestCard({ item }: RequestCardProps) {
  const location = item.location ? inboxLocationLines(item.location) : [];
  const heading = inboxHeading(item);

  return (
    <AppCard
      sx={{
        height: "100%",
        "&:hover": { borderColor: "primary.light" },
      }}
    >
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          spacing={1}
          justifyContent="space-between"
          alignItems="flex-start"
          sx={{ flexWrap: "wrap" }}
          useFlexGap
        >
          <StatusChip label={requestKindLabel(item.requestType)} tone="info" />
          <RequestStatusChip status={item.status} />
        </Stack>
        <Typography variant="h6">{heading}</Typography>
        {item.serviceSummary?.trim() ? (
          <Typography variant="caption" color="text.secondary">
            {t("provider.requests.requestId", { id: item.requestId })}
          </Typography>
        ) : null}
        {item.categorySummary ? (
          <Typography variant="body2" color="text.secondary">
            {t("provider.requests.categorySummary", { summary: item.categorySummary })}
          </Typography>
        ) : null}
        {location.length > 0 ? (
          <Stack spacing={0.25}>
            <Typography variant="caption" color="text.secondary">
              {t("provider.requests.location")}
            </Typography>
            {location.map((line) => (
              <Typography key={line} variant="body2">
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
          to={`/provider/requests/${item.requestId}`}
          variant="contained"
          sx={{ mt: "auto", minHeight: 48, width: { xs: "100%", sm: "auto" }, alignSelf: { sm: "flex-start" } }}
        >
          {t("provider.requests.viewRequest")}
        </Button>
      </Stack>
    </AppCard>
  );
}
