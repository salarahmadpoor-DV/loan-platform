/** Request kinds supported by Matchi. UI must not assume Service-only. */
export const REQUEST_KINDS = ["Service", "Product", "Hybrid"] as const;

export type RequestKind = (typeof REQUEST_KINDS)[number];

export const PROPOSER_TYPES = ["Provider", "Business"] as const;

export type ProposerType = (typeof PROPOSER_TYPES)[number];
