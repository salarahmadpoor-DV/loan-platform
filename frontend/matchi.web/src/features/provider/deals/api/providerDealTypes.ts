/** Live GET /api/provider/deals (`ProviderDealDto`). */
export type ProviderDeal = {
  id: number;
  requestId: number;
  proposalId: number;
  status: string;
  totalPrice: number;
  acceptedAt: string;
};
