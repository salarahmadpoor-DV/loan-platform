import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { DealCard } from "../components/DealCard";
import { useMyDeals } from "../hooks/useMyDeals";

export function DealListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyDeals();

  return (
    <>
      <PageHeader title={t("deal.list.title")} description={t("deal.list.description")} />
      {isPending ? <LoadingState label={t("deal.list.loading")} /> : null}
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
            {t("deal.list.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState title={t("deal.list.emptyTitle")} body={t("deal.list.emptyBody")} />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((deal) => (
            <DealCard key={deal.id} deal={deal} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
