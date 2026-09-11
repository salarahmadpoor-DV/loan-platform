import { getJson } from "../../../../shared/api/httpClient";
import type { ProviderExecution } from "./providerExecutionTypes";

export function getMyProviderExecutions(): Promise<ProviderExecution[]> {
  return getJson<ProviderExecution[]>("/api/provider/executions");
}
