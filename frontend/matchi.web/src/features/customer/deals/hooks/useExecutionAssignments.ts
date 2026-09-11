import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getExecutionAssignments } from "../api/dealsApi";

export function useExecutionAssignments(executionId: number | undefined) {
  return useQuery({
    queryKey:
      executionId != null
        ? queryKeys.executions.assignments(executionId)
        : queryKeys.executions.all,
    queryFn: () => getExecutionAssignments(executionId as number),
    enabled: executionId != null && Number.isFinite(executionId) && executionId > 0,
  });
}
