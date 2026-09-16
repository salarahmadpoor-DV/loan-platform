import { getLocale, t } from "../../../../shared/i18n";

export function isPendingProposal(status: string): boolean {
  return status.toLowerCase() === "pending";
}

export function proposalStatusLabel(status: string): string {
  const normalized = status.toLowerCase();
  if (normalized === "pending") {
    return t("proposal.status.Pending");
  }
  if (normalized === "accepted") {
    return t("proposal.status.Accepted");
  }
  if (normalized === "rejected") {
    return t("proposal.status.Rejected");
  }
  return status;
}

export function proposerPartyLabel(proposerType: string, proposerId: number): string {
  const typeLabel =
    proposerType === "Business"
      ? t("matching.type.Business")
      : proposerType === "Provider"
        ? t("matching.type.Provider")
        : proposerType;
  return t("proposal.party", { type: typeLabel, id: proposerId });
}

export function formatMoney(value: number): string {
  return new Intl.NumberFormat(getLocale(), {
    maximumFractionDigits: 0,
  }).format(value);
}

export function formatDateTime(iso: string | null | undefined): string {
  if (!iso) {
    return "—";
  }
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return iso;
  }
  return date.toLocaleString(getLocale());
}

export function proposalStatusTone(status: string): "pending" | "success" | "danger" | "neutral" {
  const normalized = status.toLowerCase();
  if (normalized === "pending") {
    return "pending";
  }
  if (normalized === "accepted") {
    return "success";
  }
  if (normalized === "rejected") {
    return "danger";
  }
  return "neutral";
}

export function formatProposalSchedule(input: {
  proposedDate: string | null;
  proposedTimeFrom: string | null;
  proposedTimeTo: string | null;
}): string | null {
  if (!input.proposedDate && !input.proposedTimeFrom && !input.proposedTimeTo) {
    return null;
  }
  const time =
    input.proposedTimeFrom || input.proposedTimeTo
      ? `${input.proposedTimeFrom ?? "—"} – ${input.proposedTimeTo ?? "—"}`
      : null;
  return [input.proposedDate, time].filter(Boolean).join(" · ");
}

export function proposalItemsSubtotal(items: { totalPrice: number }[]): number {
  return items.reduce((sum, item) => sum + item.totalPrice, 0);
}

export function parsePositiveId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}
