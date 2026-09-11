import { Button, Stack } from "@mui/material";
import { useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ProposalCard } from "../components/ProposalCard";
import { useRequestProposals } from "../hooks/useRequestProposals";
import { parsePositiveId } from "../model/proposalDisplay";

export function RequestProposalsPage() {
  const { requestId: rawId } = useParams();
  const requestId = parsePositiveId(rawId);
  const { data, isPending, isError, error, refetch, isFetching } =
    useRequestProposals(requestId);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("proposal.invalidLink") })}
      />
    );
  }

  return (
    <>
      <PageHeader
        title={t("proposal.list.title")}
        description={t("proposal.list.description")}
      />
      {isPending ? <LoadingState label={t("proposal.list.loading")} /> : null}
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
            {t("proposal.list.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("proposal.list.emptyTitle")}
          body={t("proposal.list.emptyBody")}
        />
      ) : null}
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((proposal) => (
            <ProposalCard key={proposal.id} proposal={proposal} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
