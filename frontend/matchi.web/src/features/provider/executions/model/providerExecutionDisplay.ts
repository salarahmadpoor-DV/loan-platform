import { t } from "../../../../shared/i18n";

/** Provider list labels. InProgress is the live domain value; UI copy uses Started. */
export function providerExecutionStatusLabel(status: string): string {
  const key = status.toLowerCase();
  if (key === "pending") {
    return t("provider.executions.status.Pending");
  }
  if (key === "inprogress") {
    return t("provider.executions.status.Started");
  }
  if (key === "completed") {
    return t("provider.executions.status.Completed");
  }
  if (key === "cancelled") {
    return t("provider.executions.status.Cancelled");
  }
  return status;
}
