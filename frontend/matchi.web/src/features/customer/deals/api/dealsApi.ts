import { getJson } from "../../../../shared/api/httpClient";
import type {
  DealDetail,
  DealSummary,
  ExecutionAssignment,
  ServiceExecution,
} from "./dealTypes";

export function getMyDeals(): Promise<DealSummary[]> {
  return getJson<DealSummary[]>("/api/deals");
}

export function getDealById(dealId: number): Promise<DealDetail> {
  return getJson<DealDetail>(`/api/deals/${dealId}`);
}

export function getDealExecutions(dealId: number): Promise<ServiceExecution[]> {
  return getJson<ServiceExecution[]>(`/api/deals/${dealId}/executions`);
}

export function getExecutionAssignments(
  executionId: number,
): Promise<ExecutionAssignment[]> {
  return getJson<ExecutionAssignment[]>(`/api/executions/${executionId}/assignments`);
}
