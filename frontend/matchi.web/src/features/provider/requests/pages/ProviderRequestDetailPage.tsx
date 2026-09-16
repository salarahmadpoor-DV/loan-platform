import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { requestKindLabel } from "../../../customer/requests/api/requestTypes";
import { RequestStatusChip } from "../../../customer/requests/components/RequestStatusChip";
import { useProviderInboxItem } from "../hooks/useProviderInboxItem";
import { inboxHeading, inboxLocationLines } from "../model/inboxDisplay";
import type { ProviderRequestInboxItem } from "../api/providerRequestTypes";

function parseRequestId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}

export function ProviderRequestDetailPage() {
  const { requestId: rawId } = useParams();
  const requestId = parseRequestId(rawId);
  const { item, isPending, isError, error, refetch, isFetching, notFound } =
    useProviderInboxItem(requestId);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("provider.requests.invalidLink") })}
      />
    );
  }

  const backToMarketplace = (
    <Button
      component={RouterLink}
      to="/provider/requests"
      variant="outlined"
      sx={{ minHeight: 48 }}
    >
      {t("provider.requests.backToMarketplace")}
    </Button>
  );

  return (
    <Box sx={{ maxWidth: 800, mx: "auto" }}>
      <PageHeader
        title={item ? inboxHeading(item) : t("provider.requests.detailTitle")}
        description={t("provider.requests.detailDescription")}
        action={backToMarketplace}
      />

      {isPending ? <LoadingState label={t("provider.requests.loading")} /> : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ alignSelf: "flex-start", minHeight: 48 }}
          >
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}

      {notFound ? (
        <EmptyState
          title={t("provider.requests.unavailableTitle")}
          body={t("provider.requests.unavailableBody")}
          action={backToMarketplace}
        />
      ) : null}

      {item ? (
        <ProviderRequestDetailBody item={item} />
      ) : null}
    </Box>
  );
}

function ProviderRequestDetailBody({ item }: { item: ProviderRequestInboxItem }) {
  const location = item.location ? inboxLocationLines(item.location) : [];

  return (
    <Stack spacing={2}>
      <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
        <StatusChip label={requestKindLabel(item.requestType)} tone="info" />
        <RequestStatusChip status={item.status} />
        <Typography variant="caption" color="text.secondary" sx={{ alignSelf: "center" }}>
          {t("provider.requests.requestId", { id: item.requestId })}
        </Typography>
      </Stack>

      <AppCard>
        <Typography variant="subtitle2" color="text.secondary">
          {t("provider.requests.serviceHeading")}
        </Typography>
        <Typography variant="body2" sx={{ mt: 1 }}>
          {item.serviceSummary?.trim()
            ? item.serviceSummary
            : t("provider.requests.noServiceSummary")}
        </Typography>
      </AppCard>

      <AppCard>
        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
          {t("provider.requests.categoryHeading")}
        </Typography>
        <Typography variant="body2">
          {item.categorySummary?.trim() ? item.categorySummary : t("common.notSpecified")}
        </Typography>
      </AppCard>

      <AppCard>
        <Typography variant="subtitle2" gutterBottom>
          {t("provider.requests.location")}
        </Typography>
        {location.length > 0 ? (
          location.map((line) => (
            <Typography key={line} variant="body2">
              {line}
            </Typography>
          ))
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("provider.requests.noLocation")}
          </Typography>
        )}
      </AppCard>

      <Typography variant="caption" color="text.secondary">
        {t("request.detail.created", { date: formatDateTime(item.createdDate) })}
      </Typography>

      <Button
        component={RouterLink}
        to="/provider/dashboard"
        variant="text"
        sx={{ alignSelf: "flex-start", minHeight: 48 }}
      >
        {t("provider.requests.backToDashboard")}
      </Button>
    </Stack>
  );
}
