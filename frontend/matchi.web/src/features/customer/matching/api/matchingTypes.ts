import type { ProposerType } from "../../../../shared/types/marketplace";
import type { MessageKey } from "../../../../shared/i18n";

/** Live GET /api/requests/{id}/matches item (MatchResultDto). */
export type MatchResult = {
  candidateType: string;
  candidateId: number;
  displayName: string;
  score: number;
  rank: number;
};

export type MatchCandidateType = ProposerType;

export const MATCH_SCORE_SIGNALS = [
  { key: "service", points: 50, labelKey: "matching.reason.service", providerOnly: false },
  { key: "product", points: 20, labelKey: "matching.reason.product", providerOnly: false },
  { key: "capability", points: 15, labelKey: "matching.reason.capability", providerOnly: true },
  { key: "area", points: 10, labelKey: "matching.reason.area", providerOnly: false },
  { key: "availability", points: 5, labelKey: "matching.reason.availability", providerOnly: false },
] as const satisfies ReadonlyArray<{
  key: string;
  points: number;
  labelKey: MessageKey;
  providerOnly: boolean;
}>;

export function isMatchCandidateType(value: string): value is MatchCandidateType {
  return value === "Provider" || value === "Business";
}
