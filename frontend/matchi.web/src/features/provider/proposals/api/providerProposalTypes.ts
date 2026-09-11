/** Live GET /api/provider/proposals (`ProviderProposalDto`). */
export type ProviderProposal = {
  id: number;
  requestId: number;
  status: string;
  totalPrice: number;
  createDate: string;
  dealId: number | null;
};
