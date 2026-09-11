/** Live GET /api/provider/executions (`ProviderExecutionDto`). */
export type ProviderExecution = {
  id: number;
  dealId: number;
  businessId: number | null;
  status: string;
  scheduledDate: string | null;
  scheduledTimeFrom: string | null;
  scheduledTimeTo: string | null;
  startedAt: string | null;
  completedAt: string | null;
};
