import type { NavIconName } from "../../../../shared/ui/icons";
import type { ProviderDeal } from "../../deals/api/providerDealTypes";
import type { ProviderExecution } from "../../executions/api/providerExecutionTypes";
import type { ProviderInvitation } from "../../invitations/api/invitationTypes";
import type { ProviderProposal } from "../../proposals/api/providerProposalTypes";
import type { ProviderRequestInboxItem } from "../../requests/api/providerRequestTypes";
import { inboxHeading, inboxLocationLines } from "../../requests/model/inboxDisplay";

export type ProviderActionRow = {
  id: string;
  icon: NavIconName;
  title: string;
  body: string;
  to: string;
};

export type ProviderActivityRow = {
  id: string;
  title: string;
  when: string;
  to: string;
};

export function isOpenInboxStatus(status: string): boolean {
  return status.toLowerCase() === "open";
}

export function isPendingStatus(status: string): boolean {
  return status.toLowerCase() === "pending";
}

export function isInProgressExecution(status: string): boolean {
  const key = status.toLowerCase();
  return key === "pending" || key === "inprogress";
}

export function isCompletedExecution(status: string): boolean {
  return status.toLowerCase() === "completed";
}

export function buildProviderActions(input: {
  inbox: ProviderRequestInboxItem[];
  pendingProposals: ProviderProposal[];
  invitations: ProviderInvitation[];
  attentionExecutions: ProviderExecution[];
  labels: {
    requestTitle: string;
    requestBody: (count: number) => string;
    proposalTitle: string;
    proposalBody: (count: number) => string;
    inviteTitle: string;
    inviteBody: (count: number) => string;
    executionTitle: string;
    executionBody: (count: number) => string;
  };
}): ProviderActionRow[] {
  const rows: ProviderActionRow[] = [];
  if (input.inbox.length > 0) {
    rows.push({
      id: "inbox",
      icon: "requests",
      title: input.labels.requestTitle,
      body: input.labels.requestBody(input.inbox.length),
      to: "/provider/requests",
    });
  }
  if (input.pendingProposals.length > 0) {
    rows.push({
      id: "proposals",
      icon: "reviews",
      title: input.labels.proposalTitle,
      body: input.labels.proposalBody(input.pendingProposals.length),
      to: "/provider/proposals",
    });
  }
  if (input.invitations.length > 0) {
    rows.push({
      id: "invites",
      icon: "team",
      title: input.labels.inviteTitle,
      body: input.labels.inviteBody(input.invitations.length),
      to: "/provider/invitations",
    });
  }
  if (input.attentionExecutions.length > 0) {
    rows.push({
      id: "executions",
      icon: "deals",
      title: input.labels.executionTitle,
      body: input.labels.executionBody(input.attentionExecutions.length),
      to: "/provider/executions",
    });
  }
  return rows;
}

export function requestRowMeta(item: ProviderRequestInboxItem): string {
  const location = item.location ? inboxLocationLines(item.location).join(" · ") : "";
  const parts = [item.requestType, location].filter((part) => part && part.trim().length > 0);
  return parts.join(" · ");
}

export function requestRowTitle(item: ProviderRequestInboxItem): string {
  return inboxHeading(item);
}

export function buildRecentActivity(input: {
  proposals: ProviderProposal[];
  deals: ProviderDeal[];
  executions: ProviderExecution[];
  labels: {
    proposal: (id: number) => string;
    deal: (id: number) => string;
    execution: (id: number) => string;
  };
}): ProviderActivityRow[] {
  const rows: ProviderActivityRow[] = [
    ...input.proposals.map((item) => ({
      id: `proposal-${item.id}`,
      title: input.labels.proposal(item.id),
      when: item.createDate,
      to: "/provider/proposals",
    })),
    ...input.deals.map((item) => ({
      id: `deal-${item.id}`,
      title: input.labels.deal(item.id),
      when: item.acceptedAt,
      to: "/provider/deals",
    })),
    ...input.executions.map((item) => ({
      id: `execution-${item.id}`,
      title: input.labels.execution(item.id),
      when: item.completedAt ?? item.startedAt ?? item.scheduledDate ?? "",
      to: "/provider/executions",
    })),
  ];
  return rows
    .filter((row) => row.when)
    .sort((a, b) => Date.parse(b.when) - Date.parse(a.when))
    .slice(0, 5);
}
