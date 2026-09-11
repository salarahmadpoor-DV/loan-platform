import { Button, Stack } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { RequestCard } from "../components/RequestCard";
import { useMyRequests } from "../hooks/useMyRequests";

export function RequestListPage() {
  const { data, isPending, isError, error } = useMyRequests();

  return (
    <>
      <PageHeader
        title={t("request.list.title")}
        description={t("request.list.description")}
      />
      <Button
        component={RouterLink}
        to="/customer/requests/create"
        variant="contained"
        sx={{ mb: 2 }}
      >
        {t("request.list.create")}
      </Button>
      {isPending ? <LoadingState label={t("request.list.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("request.list.emptyTitle")}
          body={t("request.list.emptyBody")}
        />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((request) => (
            <RequestCard key={request.id} request={request} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
