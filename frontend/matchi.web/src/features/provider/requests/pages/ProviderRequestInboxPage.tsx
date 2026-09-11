import { Box, Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { REQUEST_KINDS } from "../../../../shared/types/marketplace";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel } from "../../../customer/requests/api/requestTypes";
import { RequestCard } from "../components/RequestCard";
import { useProviderRequestInbox } from "../hooks/useProviderRequestInbox";

export function ProviderRequestInboxPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useProviderRequestInbox();

  return (
    <>
      <PageHeader
        title={t("provider.requests.title")}
        description={t("provider.requests.description")}
      />
      <Stack direction="row" spacing={1} sx={{ mb: 2, flexWrap: "wrap" }} useFlexGap>
        {REQUEST_KINDS.map((kind) => (
          <StatusChip key={kind} label={requestKindLabel(kind)} />
        ))}
      </Stack>
      {isPending ? <LoadingState label={t("provider.requests.loading")} /> : null}
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
            {t("provider.requests.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("provider.requests.emptyTitle")}
          body={t("provider.requests.emptyBody")}
        />
      ) : null}
      {data && data.length > 0 ? (
        <Box
          sx={{
            display: "grid",
            gap: 2,
            gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" },
          }}
        >
          {data.map((item) => (
            <RequestCard key={item.requestId} item={item} />
          ))}
        </Box>
      ) : null}
    </>
  );
}
