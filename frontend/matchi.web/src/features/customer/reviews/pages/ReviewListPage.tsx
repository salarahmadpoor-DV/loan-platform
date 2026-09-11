import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { DealCard } from "../../deals/components/DealCard";
import { useMyDeals } from "../../deals/hooks/useMyDeals";

export function ReviewListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyDeals();

  return (
    <>
      <PageHeader title={t("review.page.title")} description={t("review.page.description")} />
      {isPending ? <LoadingState label={t("review.page.loading")} /> : null}
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
            {t("review.page.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState title={t("review.page.emptyTitle")} body={t("review.page.emptyBody")} />
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
