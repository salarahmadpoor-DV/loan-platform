import { Alert, Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useLocation, useParams } from "react-router-dom";
import { formatDateTime, parsePositiveId } from "../../../../customer/proposals/model/proposalDisplay";
import { requestKindLabel } from "../../../../customer/requests/api/requestTypes";
import { RequestStatusChip } from "../../../../customer/requests/components/RequestStatusChip";
import { isRequestOpen } from "../../../../customer/requests/model/requestPresentation";
import { t } from "../../../../../shared/i18n";
import { AppCard } from "../../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../../shared/ui/StatusChip";
import { useProviderRequestInbox } from "../../../requests/hooks/useProviderRequestInbox";
import { inboxHeading, inboxLocationLines } from "../../../requests/model/inboxDisplay";
import type { ProviderRequestInboxItem } from "../../../requests/api/providerRequestTypes";
import { CreateProposalForm } from "../components/CreateProposalForm";
import { useCreateProviderProposal } from "../hooks/useCreateProviderProposal";
import { useMyProviderProducts, useMyProviderServices } from "../hooks/useProviderCatalog";
import { mapProposalApiFieldErrors } from "../model/createProposalForm";

type LocationState = {
  inboxItem?: ProviderRequestInboxItem;
};

export function CreateProviderProposalPage() {
  const requestId = parsePositiveId(useParams().requestId);
  const locationItem = (useLocation().state as LocationState | null)?.inboxItem;
  const inbox = useProviderRequestInbox();
  const services = useMyProviderServices();
  const products = useMyProviderProducts();
  const create = useCreateProviderProposal(requestId ?? 0);

  const inboxItem =
    locationItem && locationItem.requestId === requestId
      ? locationItem
      : inbox.data?.find((item) => item.requestId === requestId);

  const requestPath =
    requestId != null ? `/provider/requests/${requestId}` : "/provider/requests";
  const canSubmit = !inboxItem || isRequestOpen(inboxItem.status);
  const serverErrors = create.isError ? mapProposalApiFieldErrors(create.error) : undefined;

  if (requestId == null) {
    return (
      <>
        <PageHeader title={t("provider.proposalCreate.title")} />
        <EmptyState
          title={t("provider.proposalCreate.invalid")}
          action={
            <Button component={RouterLink} to="/provider/requests" variant="outlined">
              {t("provider.proposalCreate.backToMarketplace")}
            </Button>
          }
        />
      </>
    );
  }

  if (create.isSuccess) {
    return (
      <Box sx={{ maxWidth: 640, mx: "auto" }}>
        <PageHeader title={t("provider.proposalCreate.successTitle")} />
        <Box role="status">
        <EmptyState
          title={t("provider.proposalCreate.successTitle")}
          body={t("provider.proposalCreate.successBody", { id: create.data.proposalId })}
          action={
            <Stack direction={{ xs: "column", sm: "row" }} spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
              <Button component={RouterLink} to={requestPath} variant="outlined" sx={{ minHeight: 48 }}>
                {t("provider.proposalCreate.backToRequest")}
              </Button>
              <Button
                component={RouterLink}
                to="/provider/requests"
                variant="outlined"
                sx={{ minHeight: 48 }}
              >
                {t("provider.proposalCreate.backToMarketplace")}
              </Button>
              <Button component={RouterLink} to="/provider/proposals" variant="contained" sx={{ minHeight: 48 }}>
                {t("provider.proposalCreate.viewMine")}
              </Button>
            </Stack>
          }
        />
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ maxWidth: 960, mx: "auto" }}>
      <PageHeader
        title={t("provider.proposalCreate.title")}
        description={t("provider.proposalCreate.description")}
        action={
          <Button component={RouterLink} to={requestPath} variant="outlined" sx={{ minHeight: 48 }}>
            {t("provider.proposalCreate.backToRequest")}
          </Button>
        }
      />

      {inbox.isPending && !inboxItem ? <LoadingState label={t("provider.requests.loading")} /> : null}

      <AppCard>
        <Stack spacing={1}>
          <Typography variant="subtitle1">
            {inboxItem ? inboxHeading(inboxItem) : t("provider.requests.requestId", { id: requestId })}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t("provider.proposalCreate.body", { id: requestId })}
          </Typography>
          {inboxItem ? (
            <>
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                <StatusChip label={requestKindLabel(inboxItem.requestType)} tone="info" />
                <RequestStatusChip status={inboxItem.status} />
              </Stack>
              <Typography variant="body2" sx={{ mt: 0.5 }}>
                {inboxItem.serviceSummary?.trim()
                  ? inboxItem.serviceSummary
                  : t("provider.requests.noServiceSummary")}
              </Typography>
              {inboxItem.categorySummary ? (
                <Typography variant="body2" color="text.secondary">
                  {t("provider.requests.categorySummary", { summary: inboxItem.categorySummary })}
                </Typography>
              ) : null}
              {inboxItem.location && inboxLocationLines(inboxItem.location).length > 0 ? (
                <Typography variant="body2" color="text.secondary">
                  {inboxLocationLines(inboxItem.location).join(" · ")}
                </Typography>
              ) : null}
              {inboxItem.createdDate ? (
                <Typography variant="caption" color="text.secondary">
                  {t("request.detail.created", { date: formatDateTime(inboxItem.createdDate) })}
                </Typography>
              ) : null}
            </>
          ) : (
            <Typography variant="body2" color="text.secondary">
              {t("provider.proposalCreate.contextMissing")}
            </Typography>
          )}
        </Stack>
      </AppCard>

      {!canSubmit ? (
        <Alert severity="warning" sx={{ mt: 2 }} role="status">
          {t("provider.proposalCreate.notOpen")}
        </Alert>
      ) : null}

      {create.isError ? (
        <Stack sx={{ mt: 2 }}>
          <ErrorAlert error={create.error} />
        </Stack>
      ) : null}

      {canSubmit ? (
        <Stack sx={{ mt: 2 }}>
          <CreateProposalForm
            requestType={inboxItem?.requestType}
            services={services.data ?? []}
            products={products.data ?? []}
            servicesLoading={services.isPending}
            productsLoading={products.isPending}
            servicesFailed={services.isError}
            productsFailed={products.isError}
            submitting={create.isPending}
            serverErrors={serverErrors}
            onSubmit={(body) => {
              if (create.isPending) {
                return;
              }
              create.mutate(body);
            }}
          />
        </Stack>
      ) : null}
    </Box>
  );
}
