/** Live GET /api/deals list (`DealSummaryDto`). */
export type DealSummary = {
  id: number;
  requestId: number;
  proposalId: number;
  status: string;
  totalPrice: number;
  acceptedAt: string;
};

/** Live GET /api/deals/{dealId} (`DealDetailDto`). */
export type DealDetail = {
  id: number;
  requestId: number;
  proposalId: number;
  customerId: number;
  status: string;
  totalPrice: number;
  acceptedAt: string;
  createDate: string;
  updateDate: string | null;
};

/**
 * Live GET /api/deals/{dealId}/executions (`ServiceExecutionDto`).
 * Assignments are not nested; they are a separate GET.
 */
export type ServiceExecution = {
  id: number;
  dealId: number;
  businessId: number | null;
  status: string;
  scheduledDate: string | null;
  scheduledTimeFrom: string | null;
  scheduledTimeTo: string | null;
  startedAt: string | null;
  completedAt: string | null;
  createDate: string;
};

/** Live GET /api/executions/{executionId}/assignments (`ExecutionAssignmentDto`). */
export type ExecutionAssignment = {
  id: number;
  serviceExecutionId: number;
  providerId: number;
  role: string;
  isPrimary: boolean;
  status: string;
  assignedAt: string;
};
