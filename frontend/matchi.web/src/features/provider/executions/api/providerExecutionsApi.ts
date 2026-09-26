import { getJson, postJson } from "../../../../shared/api/httpClient";
import type { ProviderExecution } from "./providerExecutionTypes";

export function getMyProviderExecutions(): Promise<ProviderExecution[]> {
  return getJson<ProviderExecution[]>("/api/provider/executions");
}

export function startProviderExecution(executionId: number): Promise<{ executionId: number; status: string }> {
  return postJson(`/api/executions/${executionId}/start`);
}

export function completeProviderExecution(
  executionId: number,
): Promise<{ executionId: number; status: string }> {
  return postJson(`/api/executions/${executionId}/complete`);
}
