import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ProviderProposalCard } from "../components/ProviderProposalCard";
import { useMyProviderProposals } from "../hooks/useMyProviderProposals";

export function ProviderProposalListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderProposals();

  return (
    <>
      <PageHeader
        title={t("provider.proposals.title")}
        description={t("provider.proposals.description")}
      />
      {isPending ? <LoadingState label={t("provider.proposals.loading")} /> : null}
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
            {t("provider.proposals.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("provider.proposals.emptyTitle")}
          body={t("provider.proposals.emptyBody")}
        />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((proposal) => (
            <ProviderProposalCard key={proposal.id} proposal={proposal} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
