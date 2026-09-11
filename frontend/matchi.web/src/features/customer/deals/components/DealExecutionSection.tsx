import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { useDealExecutions } from "../hooks/useDealExecutions";
import { ExecutionCard } from "./ExecutionCard";

type DealExecutionSectionProps = {
  dealId: number;
  requestType: string | undefined;
};

export function DealExecutionSection({ dealId, requestType }: DealExecutionSectionProps) {
  const { data, isPending, isError, error, refetch, isFetching } = useDealExecutions(dealId);

  return (
    <Stack spacing={1.5}>
      <Typography variant="subtitle1">{t("deal.detail.execution")}</Typography>
      {isPending ? <LoadingState label={t("deal.execution.loading")} /> : null}
      {isError ? (
        <Stack spacing={1}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ alignSelf: "flex-start" }}
          >
            {t("deal.execution.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && (data?.length ?? 0) === 0 ? (
        <EmptyState
          title={t("deal.detail.noExecution")}
          body={
            requestType === "Product"
              ? t("deal.execution.emptyProduct")
              : requestType === "Service" || requestType === "Hybrid"
                ? t("deal.execution.emptyService")
                : undefined
          }
        />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={1.5}>
          {data.map((execution) => (
            <ExecutionCard key={execution.id} execution={execution} />
          ))}
        </Stack>
      ) : null}
    </Stack>
  );
}
