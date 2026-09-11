export type CreateProviderProposalItemBody = {
  itemType: "Service" | "Product";
  productId: number | null;
  serviceId: number | null;
  description?: string | null;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  displayOrder: number;
};

/** Live POST /api/requests/{requestId}/proposals body for a Provider party. */
export type CreateProviderProposalBody = {
  proposerType: "Provider";
  totalPrice: number;
  deliveryFee: number;
  message?: string | null;
  proposedDate?: string | null;
  proposedTimeFrom?: string | null;
  proposedTimeTo?: string | null;
  expireAt?: string | null;
  items: CreateProviderProposalItemBody[];
};

export type CreateProviderProposalResult = {
  proposalId: number;
};
