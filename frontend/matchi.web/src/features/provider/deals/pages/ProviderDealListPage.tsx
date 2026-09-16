import { Button, Stack, Typography } from "@mui/material";
import { dealStatusLabel } from "../../../customer/deals/model/dealDisplay";
import { t } from "../../../../shared/i18n";
import { groupByStatus } from "../../../../shared/marketplace/groupByStatus";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { ProviderDealCard } from "../components/ProviderDealCard";
import { useMyProviderDeals } from "../hooks/useMyProviderDeals";

export function ProviderDealListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderDeals();
  const groups = groupByStatus(data ?? [], (item) => item.status, [
    "Active",
    "Completed",
    "Cancelled",
  ]);

  return (
    <>
      <PageHeader title={t("provider.deals.title")} description={t("provider.deals.description")} />
      {isPending ? <LoadingState label={t("provider.deals.loading")} /> : null}
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
            {t("provider.deals.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState title={t("provider.deals.emptyTitle")} body={t("provider.deals.emptyBody")} />
      ) : null}
      {groups.map((group) => (
        <Stack key={group.status} spacing={1.5} sx={{ mb: 3 }}>
          <Typography variant="h6">{dealStatusLabel(group.status)}</Typography>
          <ResponsiveCardGrid>
            {group.items.map((deal) => (
              <ProviderDealCard key={deal.id} deal={deal} />
            ))}
          </ResponsiveCardGrid>
        </Stack>
      ))}
    </>
  );
}
