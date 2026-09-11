/**
 * Query key factory. Feature modules should extend these namespaces
 * rather than inventing ad-hoc keys.
 */
export const queryKeys = {
  auth: {
    session: ["auth", "session"] as const,
  },
  catalog: {
    services: ["catalog", "services"] as const,
    products: ["catalog", "products"] as const,
    businesses: ["catalog", "businesses"] as const,
    providers: ["catalog", "providers"] as const,
  },
  requests: {
    all: ["requests"] as const,
    mine: () => [...queryKeys.requests.all, "me"] as const,
    detail: (requestId: number) => [...queryKeys.requests.all, requestId] as const,
  },
  matching: {
    all: ["matching"] as const,
    byRequest: (requestId: number) =>
      [...queryKeys.matching.all, "request", requestId] as const,
  },
  proposals: {
    all: ["proposals"] as const,
    byRequest: (requestId: number) =>
      [...queryKeys.proposals.all, "request", requestId] as const,
    detail: (proposalId: number) => [...queryKeys.proposals.all, proposalId] as const,
  },
  deals: {
    all: ["deals"] as const,
    mine: () => [...queryKeys.deals.all, "me"] as const,
    detail: (dealId: number) => [...queryKeys.deals.all, dealId] as const,
  },
  executions: {
    all: ["executions"] as const,
    byDeal: (dealId: number) => [...queryKeys.executions.all, "deal", dealId] as const,
    assignments: (executionId: number) =>
      [...queryKeys.executions.all, executionId, "assignments"] as const,
  },
  productDelivery: {
    all: ["productDelivery"] as const,
  },
  reviews: {
    all: ["reviews"] as const,
    byProvider: (providerId: number) =>
      [...queryKeys.reviews.all, "provider", providerId] as const,
    byBusiness: (businessId: number) =>
      [...queryKeys.reviews.all, "business", businessId] as const,
  },
  provider: {
    all: ["provider"] as const,
    profile: () => [...queryKeys.provider.all, "me"] as const,
    requests: () => [...queryKeys.provider.all, "requests"] as const,
    proposals: () => [...queryKeys.provider.all, "proposals"] as const,
    deals: () => [...queryKeys.provider.all, "deals"] as const,
    executions: () => [...queryKeys.provider.all, "executions"] as const,
    myServices: () => [...queryKeys.provider.all, "me", "services"] as const,
    myProducts: () => [...queryKeys.provider.all, "me", "products"] as const,
  },
} as const;
