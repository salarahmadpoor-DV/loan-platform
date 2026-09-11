import { t } from "../../../../shared/i18n";

export function dealStatusLabel(status: string): string {
  if (status.toLowerCase() === "active") {
    return t("deal.status.Active");
  }
  return status;
}

export function executionStatusLabel(status: string): string {
  const key = status.toLowerCase();
  if (key === "pending") {
    return t("deal.execution.status.Pending");
  }
  if (key === "inprogress") {
    return t("deal.execution.status.InProgress");
  }
  if (key === "completed") {
    return t("deal.execution.status.Completed");
  }
  if (key === "cancelled") {
    return t("deal.execution.status.Cancelled");
  }
  return status;
}

export function assignmentStatusLabel(status: string): string {
  const key = status.toLowerCase();
  if (key === "assigned") {
    return t("deal.assignment.status.Assigned");
  }
  if (key === "cancelled") {
    return t("deal.assignment.status.Cancelled");
  }
  return status;
}

export function formatExecutionSchedule(parts: Array<string | null | undefined>): string | null {
  const values = parts.filter((part): part is string => Boolean(part));
  return values.length > 0 ? values.join(" — ") : null;
}

export function parsePositiveId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}
