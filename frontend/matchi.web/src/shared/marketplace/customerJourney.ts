export type JourneyStepId =
  | "request"
  | "matching"
  | "proposal"
  | "deal"
  | "execution"
  | "review";

export type JourneyStepState = "complete" | "current" | "upcoming" | "notApplicable" | "unknown";

export type CustomerJourneyFacts = {
  current: JourneyStepId;
  requestId?: number;
  dealId?: number;
  requestExists?: boolean;
  matchesLoaded?: boolean;
  hasMatches?: boolean;
  proposalsLoaded?: boolean;
  hasProposals?: boolean;
  proposalAccepted?: boolean;
  dealExists?: boolean;
  requestType?: string;
  executionsLoaded?: boolean;
  hasExecution?: boolean;
  executionCompleted?: boolean;
};

export type JourneyStepView = {
  id: JourneyStepId;
  state: JourneyStepState;
  href?: string;
};

const ORDER: JourneyStepId[] = [
  "request",
  "matching",
  "proposal",
  "deal",
  "execution",
  "review",
];

function isProductRequest(requestType: string | undefined): boolean {
  return requestType === "Product";
}

/**
 * Completes a step only when the live API supports that conclusion.
 * Matching is complete only after matches were loaded and the list is non-empty.
 * Review is never marked complete from the public review list (no dealId on ReviewDto).
 */
export function buildCustomerJourney(facts: CustomerJourneyFacts): JourneyStepView[] {
  const requestHref =
    facts.requestId != null ? `/customer/requests/${facts.requestId}` : undefined;
  const matchingHref =
    facts.requestId != null ? `/customer/requests/${facts.requestId}/matches` : undefined;
  const proposalHref =
    facts.requestId != null ? `/customer/requests/${facts.requestId}/proposals` : undefined;
  const dealHref = facts.dealId != null ? `/customer/deals/${facts.dealId}` : undefined;

  const matchingState: JourneyStepState = (() => {
    if (facts.current === "matching") {
      return "current";
    }
    if (facts.matchesLoaded && facts.hasMatches) {
      return "complete";
    }
    if (facts.matchesLoaded && !facts.hasMatches) {
      return "unknown";
    }
    return facts.current === "request" ? "upcoming" : "unknown";
  })();

  const proposalState: JourneyStepState = (() => {
    if (facts.current === "proposal") {
      return "current";
    }
    if (facts.proposalAccepted || facts.dealExists) {
      return "complete";
    }
    if (facts.proposalsLoaded && facts.hasProposals) {
      return ORDER.indexOf(facts.current) > ORDER.indexOf("proposal") ? "complete" : "unknown";
    }
    return ORDER.indexOf(facts.current) < ORDER.indexOf("proposal") ? "upcoming" : "unknown";
  })();

  const dealState: JourneyStepState = (() => {
    if (facts.current === "deal") {
      return "current";
    }
    if (facts.dealExists) {
      return "complete";
    }
    return ORDER.indexOf(facts.current) < ORDER.indexOf("deal") ? "upcoming" : "unknown";
  })();

  const executionState: JourneyStepState = (() => {
    if (isProductRequest(facts.requestType)) {
      return "notApplicable";
    }
    if (facts.current === "execution") {
      return "current";
    }
    if (facts.executionsLoaded && facts.executionCompleted) {
      return "complete";
    }
    if (facts.executionsLoaded && facts.hasExecution) {
      return ORDER.indexOf(facts.current) >= ORDER.indexOf("execution") ? "current" : "unknown";
    }
    if (facts.executionsLoaded && !facts.hasExecution) {
      return "unknown";
    }
    return ORDER.indexOf(facts.current) < ORDER.indexOf("execution") ? "upcoming" : "unknown";
  })();

  const reviewState: JourneyStepState =
    facts.current === "review" ? "current" : "upcoming";

  const requestState: JourneyStepState =
    facts.current === "request"
      ? "current"
      : facts.requestExists
        ? "complete"
        : "unknown";

  const hrefFor = (id: JourneyStepId): string | undefined => {
    if (id === "request") {
      return requestHref;
    }
    if (id === "matching") {
      return matchingHref;
    }
    if (id === "proposal") {
      return proposalHref;
    }
    if (id === "deal" || id === "execution" || id === "review") {
      return dealHref;
    }
    return undefined;
  };

  return ORDER.map((id) => {
    const stateById: Record<JourneyStepId, JourneyStepState> = {
      request: requestState,
      matching: matchingState,
      proposal: proposalState,
      deal: dealState,
      execution: executionState,
      review: reviewState,
    };
    return { id, state: stateById[id], href: hrefFor(id) };
  });
}
