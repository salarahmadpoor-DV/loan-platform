import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ProviderDealCard } from "../components/ProviderDealCard";
import { useMyProviderDeals } from "../hooks/useMyProviderDeals";

export function ProviderDealListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderDeals();

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
            sx={{ alignSelf: "flex-start" }}
          >
            {t("provider.deals.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState title={t("provider.deals.emptyTitle")} body={t("provider.deals.emptyBody")} />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((deal) => (
            <ProviderDealCard key={deal.id} deal={deal} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
