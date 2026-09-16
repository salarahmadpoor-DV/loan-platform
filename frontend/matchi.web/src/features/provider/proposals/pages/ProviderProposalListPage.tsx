import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { groupByStatus } from "../../../../shared/marketplace/groupByStatus";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { proposalStatusLabel } from "../../../customer/proposals/model/proposalDisplay";
import { ProviderProposalCard } from "../components/ProviderProposalCard";
import { useMyProviderProposals } from "../hooks/useMyProviderProposals";

export function ProviderProposalListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderProposals();
  const groups = groupByStatus(data ?? [], (item) => item.status, [
    "Pending",
    "Accepted",
    "Rejected",
  ]);

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
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
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
      {groups.map((group) => (
        <Stack key={group.status} spacing={1.5} sx={{ mb: 3 }}>
          <Typography variant="h6">{proposalStatusLabel(group.status)}</Typography>
          <ResponsiveCardGrid>
            {group.items.map((proposal) => (
              <ProviderProposalCard key={proposal.id} proposal={proposal} />
            ))}
          </ResponsiveCardGrid>
        </Stack>
      ))}
    </>
  );
}
