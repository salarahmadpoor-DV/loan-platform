import { Alert, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useLocation, useParams } from "react-router-dom";
import { parsePositiveId } from "../../../../customer/proposals/model/proposalDisplay";
import { requestKindLabel } from "../../../../customer/requests/api/requestTypes";
import { t } from "../../../../../shared/i18n";
import { AppCard } from "../../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../../shared/ui/ErrorAlert";
import { PageHeader } from "../../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../../shared/ui/StatusChip";
import { useProviderRequestInbox } from "../../../requests/hooks/useProviderRequestInbox";
import type { ProviderRequestInboxItem } from "../../../requests/api/providerRequestTypes";
import { CreateProposalForm } from "../components/CreateProposalForm";
import { useCreateProviderProposal } from "../hooks/useCreateProviderProposal";
import { useMyProviderProducts, useMyProviderServices } from "../hooks/useProviderCatalog";

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

  if (requestId == null) {
    return (
      <>
        <PageHeader title={t("provider.proposalCreate.title")} />
        <EmptyState
          title={t("provider.proposalCreate.invalid")}
          action={
            <Button component={RouterLink} to="/provider/requests" variant="outlined">
              {t("provider.proposalCreate.back")}
            </Button>
          }
        />
      </>
    );
  }

  if (create.isSuccess) {
    return (
      <>
        <PageHeader title={t("provider.proposalCreate.title")} />
        <EmptyState
          title={t("provider.proposalCreate.successTitle")}
          body={t("provider.proposalCreate.successBody", { id: create.data.proposalId })}
          action={
            <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
              <Button component={RouterLink} to="/provider/requests" variant="outlined">
                {t("provider.proposalCreate.back")}
              </Button>
              <Button component={RouterLink} to="/provider/proposals" variant="contained">
                {t("provider.proposalCreate.viewMine")}
              </Button>
            </Stack>
          }
        />
      </>
    );
  }

  return (
    <>
      <PageHeader
        title={t("provider.proposalCreate.title")}
        description={t("provider.proposalCreate.description")}
      />
      <AppCard>
        <Stack spacing={1}>
          <Typography variant="subtitle1">
            {t("provider.requests.requestId", { id: requestId })}
          </Typography>
          {inboxItem ? (
            <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
              <StatusChip label={requestKindLabel(inboxItem.requestType)} />
              <StatusChip label={inboxItem.status} />
            </Stack>
          ) : (
            <Typography variant="body2" color="text.secondary">
              {t("provider.proposalCreate.contextMissing")}
            </Typography>
          )}
          {inboxItem?.serviceSummary ? (
            <Typography variant="body2">
              {t("provider.requests.serviceSummary", { summary: inboxItem.serviceSummary })}
            </Typography>
          ) : null}
          {inboxItem?.categorySummary ? (
            <Typography variant="body2" color="text.secondary">
              {t("provider.requests.categorySummary", { summary: inboxItem.categorySummary })}
            </Typography>
          ) : null}
        </Stack>
      </AppCard>

      {inboxItem && inboxItem.status.toLowerCase() !== "open" ? (
        <Alert severity="warning" sx={{ mt: 2 }}>
          {t("provider.proposalCreate.notOpen")}
        </Alert>
      ) : null}

      {create.isError ? (
        <Stack sx={{ mt: 2 }}>
          <ErrorAlert error={create.error} />
        </Stack>
      ) : null}

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
          onSubmit={(body) => {
            if (create.isPending) {
              return;
            }
            create.mutate(body);
          }}
        />
      </Stack>
    </>
  );
}
