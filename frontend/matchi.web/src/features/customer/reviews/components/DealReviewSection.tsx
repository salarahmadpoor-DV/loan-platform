import { Stack, Typography } from "@mui/material";
import { useQueries } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import type { DealDetail, ExecutionAssignment } from "../../deals/api/dealTypes";
import { getExecutionAssignments } from "../../deals/api/dealsApi";
import { useDealExecutions } from "../../deals/hooks/useDealExecutions";
import { useProposal } from "../../proposals/hooks/useProposal";
import { useRequest } from "../../requests/hooks/useRequest";
import { getReviewEligibility } from "../model/reviewEligibility";
import { reviewTargetsFromDealParty } from "../model/reviewTargets";
import { CreateReviewForm } from "./CreateReviewForm";
import { ReviewEligibilityInfo } from "./ReviewEligibilityInfo";
import { ReviewList } from "./ReviewList";

type DealReviewSectionProps = {
  deal: DealDetail;
};

export function DealReviewSection({ deal }: DealReviewSectionProps) {
  const requestQuery = useRequest(deal.requestId);
  const proposalQuery = useProposal(deal.proposalId);
  const executionsQuery = useDealExecutions(deal.id);
  const executions = executionsQuery.data ?? [];

  const assignmentQueries = useQueries({
    queries: executions.map((execution) => ({
      queryKey: queryKeys.executions.assignments(execution.id),
      queryFn: () => getExecutionAssignments(execution.id),
      enabled: executionsQuery.isSuccess,
    })),
  });

  const assignments: ExecutionAssignment[] = assignmentQueries.flatMap(
    (query) => query.data ?? [],
  );
  const targets = reviewTargetsFromDealParty(proposalQuery.data, assignments);
  const listTarget = targets[0];

  const eligibility = getReviewEligibility({
    dealStatus: deal.status,
    requestType: requestQuery.data?.requestType,
    executions: executionsQuery.data,
    executionsLoaded: executionsQuery.isSuccess,
    hasTarget: targets.length > 0,
  });

  return (
    <AppCard>
      <Stack spacing={2}>
        <Typography variant="subtitle1">{t("review.section.title")}</Typography>
        <Typography variant="body2" color="text.secondary">
          {t("review.section.listNote")}
        </Typography>

        {proposalQuery.isPending || requestQuery.isPending ? (
          <LoadingState label={t("review.section.loading")} />
        ) : null}
        {proposalQuery.isError ? <ErrorAlert error={proposalQuery.error} /> : null}
        {requestQuery.isError ? <ErrorAlert error={requestQuery.error} /> : null}
        {executionsQuery.isError ? <ErrorAlert error={executionsQuery.error} /> : null}

        <ReviewEligibilityInfo eligibility={eligibility} />

        <Typography variant="subtitle2">{t("review.list.title")}</Typography>
        <ReviewList target={listTarget} />

        {eligibility.canSubmit ? (
          <CreateReviewForm dealId={deal.id} targets={targets} disabled={false} />
        ) : null}
      </Stack>
    </AppCard>
  );
}
