import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import type { ReviewTarget } from "../api/reviewTypes";
import { useTargetReviews } from "../hooks/useTargetReviews";
import { ReviewCard } from "./ReviewCard";

type ReviewListProps = {
  target: ReviewTarget | undefined;
};

export function ReviewList({ target }: ReviewListProps) {
  const { data, isPending, isError, error, refetch, isFetching } = useTargetReviews(target);

  if (target == null) {
    return <EmptyState title={t("review.list.noTarget")} />;
  }

  return (
    <Stack spacing={1.5}>
      {isPending ? <LoadingState label={t("review.list.loading")} /> : null}
      {isError ? (
        <Stack spacing={1}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            size="small"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ alignSelf: "flex-start" }}
          >
            {t("review.list.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && (data?.length ?? 0) === 0 ? (
        <EmptyState title={t("review.list.empty")} body={t("review.list.emptyBody")} />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={1}>
          {data.map((review) => (
            <ReviewCard key={review.id} review={review} />
          ))}
        </Stack>
      ) : null}
    </Stack>
  );
}
