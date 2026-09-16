import { Box, Button, Stack, ToggleButton, ToggleButtonGroup, Typography } from "@mui/material";
import { useMemo, useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { REQUEST_KINDS, type RequestKind } from "../../../../shared/types/marketplace";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { requestKindLabel } from "../../../customer/requests/api/requestTypes";
import { ProviderRequestCardSkeletonGrid } from "../components/ProviderRequestCardSkeletonGrid";
import { RequestCard } from "../components/RequestCard";
import { useProviderRequestInbox } from "../hooks/useProviderRequestInbox";

export function ProviderRequestInboxPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useProviderRequestInbox();
  const [kindFilter, setKindFilter] = useState<RequestKind | "all">("all");

  const items = useMemo(() => {
    if (!data) {
      return [];
    }
    if (kindFilter === "all") {
      return data;
    }
    return data.filter((item) => item.requestType === kindFilter);
  }, [data, kindFilter]);

  return (
    <Box sx={{ maxWidth: 960, mx: "auto" }}>
      <PageHeader
        title={t("provider.requests.title")}
        description={t("provider.requests.description")}
        action={
          <Button
            component={RouterLink}
            to="/provider/dashboard"
            variant="outlined"
            sx={{ minHeight: 48 }}
          >
            {t("provider.requests.backToDashboard")}
          </Button>
        }
      />
      <ToggleButtonGroup
        exclusive
        fullWidth
        color="primary"
        value={kindFilter}
        onChange={(_event, next: RequestKind | "all" | null) => {
          if (next) {
            setKindFilter(next);
          }
        }}
        aria-label={t("provider.requests.filterKind")}
        sx={{ mb: 2, flexWrap: "wrap" }}
      >
        <ToggleButton value="all" sx={{ py: 1.25, flex: { xs: "1 1 100%", sm: "1 1 0" } }}>
          {t("provider.requests.filterAll")}
        </ToggleButton>
        {REQUEST_KINDS.map((kind) => (
          <ToggleButton
            key={kind}
            value={kind}
            sx={{ py: 1.25, flex: { xs: "1 1 100%", sm: "1 1 0" } }}
          >
            {requestKindLabel(kind)}
          </ToggleButton>
        ))}
      </ToggleButtonGroup>
      {isPending ? (
        <Stack spacing={2} aria-busy="true" aria-live="polite">
          <LoadingState label={t("provider.requests.loading")} />
          <ProviderRequestCardSkeletonGrid />
        </Stack>
      ) : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
          >
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("provider.requests.emptyTitle")}
          body={t("provider.requests.emptyBody")}
          action={
            <Button component={RouterLink} to="/provider/dashboard" variant="contained">
              {t("provider.requests.backToDashboard")}
            </Button>
          }
        />
      ) : null}
      {!isPending && !isError && data && data.length > 0 && items.length === 0 ? (
        <EmptyState title={t("provider.requests.filterEmpty")} />
      ) : null}
      {items.length > 0 ? (
        <Stack spacing={2}>
          <Typography variant="body2" color="text.secondary">
            {t("provider.requests.resultCount", { count: items.length })}
          </Typography>
          <ResponsiveCardGrid>
            {items.map((item) => (
              <RequestCard key={item.requestId} item={item} />
            ))}
          </ResponsiveCardGrid>
        </Stack>
      ) : null}
    </Box>
  );
}
