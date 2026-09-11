import type { ExecutionAssignment } from "../../deals/api/dealTypes";
import type { ProposalDetail } from "../../proposals/api/proposalTypes";
import type { ReviewTarget } from "../api/reviewTypes";

export function reviewTargetsFromDealParty(
  proposal: ProposalDetail | undefined,
  assignments: ExecutionAssignment[],
): ReviewTarget[] {
  const targets: ReviewTarget[] = [];
  const seen = new Set<string>();

  const add = (target: ReviewTarget) => {
    const key = `${target.kind}:${target.id}`;
    if (seen.has(key)) {
      return;
    }
    seen.add(key);
    targets.push(target);
  };

  if (proposal?.proposerType === "Business") {
    add({ kind: "Business", id: proposal.proposerId });
  }
  if (proposal?.proposerType === "Provider") {
    add({ kind: "Provider", id: proposal.proposerId });
  }

  for (const assignment of assignments) {
    add({ kind: "Provider", id: assignment.providerId });
  }

  return targets;
}

export function targetKey(target: ReviewTarget): string {
  return `${target.kind}:${target.id}`;
}

export function parseTargetKey(value: string): ReviewTarget | undefined {
  const [kind, rawId] = value.split(":");
  const id = Number.parseInt(rawId ?? "", 10);
  if ((kind !== "Business" && kind !== "Provider") || !Number.isFinite(id) || id <= 0) {
    return undefined;
  }
  return { kind, id };
}
