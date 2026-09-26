import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { completeProviderExecution, startProviderExecution } from "../api/providerExecutionsApi";

function invalidateExecutionQueries(queryClient: ReturnType<typeof useQueryClient>, dealId: number) {
  void queryClient.invalidateQueries({ queryKey: queryKeys.provider.executions() });
  void queryClient.invalidateQueries({ queryKey: queryKeys.provider.deals() });
  void queryClient.invalidateQueries({ queryKey: queryKeys.executions.all });
  void queryClient.invalidateQueries({ queryKey: queryKeys.executions.byDeal(dealId) });
  void queryClient.invalidateQueries({ queryKey: queryKeys.deals.all });
}

export function useStartProviderExecution(dealId: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (executionId: number) => startProviderExecution(executionId),
    onSuccess: () => invalidateExecutionQueries(queryClient, dealId),
  });
}

export function useCompleteProviderExecution(dealId: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (executionId: number) => completeProviderExecution(executionId),
    onSuccess: () => invalidateExecutionQueries(queryClient, dealId),
  });
}
