import { formatIsoDateForLocale } from "../../../../shared/datetime/LocaleDateField";
import { getLocale, t } from "../../../../shared/i18n";
import type { StatusTone } from "../../../../shared/ui/StatusChip";

export function dealStatusLabel(status: string): string {
  const key = status.toLowerCase();
  if (key === "active") {
    return t("deal.status.Active");
  }
  if (key === "completed") {
    return t("deal.status.Completed");
  }
  if (key === "cancelled") {
    return t("deal.status.Cancelled");
  }
  return status;
}

export function dealStatusTone(status: string): StatusTone {
  const key = status.toLowerCase();
  if (key === "active") {
    return "success";
  }
  if (key === "completed") {
    return "info";
  }
  if (key === "cancelled") {
    return "danger";
  }
  return "neutral";
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
  const values = parts
    .filter((part): part is string => Boolean(part))
    .map((part) =>
      /^\d{4}-\d{2}-\d{2}$/.test(part) ? formatIsoDateForLocale(part, getLocale()) || part : part,
    );
  return values.length > 0 ? values.join(" — ") : null;
}

export function parsePositiveId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}
