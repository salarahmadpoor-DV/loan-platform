import { getLocale, t } from "../../../../shared/i18n";

export function isRequestOpen(status: string): boolean {
  return status.toLowerCase() === "open";
}

export function isRequestCancelled(status: string): boolean {
  return status.toLowerCase() === "cancelled";
}

export function requestStatusLabel(status: string): string {
  const key = status.toLowerCase();
  if (key === "open") {
    return t("request.status.Open");
  }
  if (key === "cancelled") {
    return t("request.status.Cancelled");
  }
  return status;
}

export function requestStatusTone(status: string): "success" | "neutral" | "pending" {
  if (isRequestOpen(status)) {
    return "success";
  }
  if (isRequestCancelled(status)) {
    return "neutral";
  }
  return "pending";
}

export function formatRequestDateTime(iso: string | null | undefined): string {
  if (!iso) {
    return t("common.notSpecified");
  }
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return iso;
  }
  return date.toLocaleString(getLocale());
}

export function formatRequestDate(iso: string): string {
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return iso;
  }
  return date.toLocaleDateString(getLocale());
}

export function compareMyRequests(
  a: { status: string; createDate: string },
  b: { status: string; createDate: string },
): number {
  const aRank = isRequestOpen(a.status) ? 0 : 1;
  const bRank = isRequestOpen(b.status) ? 0 : 1;
  if (aRank !== bRank) {
    return aRank - bRank;
  }
  return new Date(b.createDate).getTime() - new Date(a.createDate).getTime();
}

export function requestLocationSummary(location: {
  city: string | null;
  district: string | null;
  province: string | null;
} | null): string | null {
  if (!location) {
    return null;
  }
  const parts = [location.city, location.district, location.province].filter(
    (part): part is string => Boolean(part && part.trim()),
  );
  return parts.length > 0 ? parts.join(" · ") : null;
}

export function requestDescriptionSnippet(description: string | null, maxLength = 140): string | null {
  const text = description?.trim();
  if (!text) {
    return null;
  }
  return text.length > maxLength ? `${text.slice(0, maxLength)}…` : text;
}
