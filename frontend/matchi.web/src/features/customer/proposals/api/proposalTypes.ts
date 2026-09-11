export type ProposalListItem = {
  id: number;
  requestId: number;
  proposerType: string;
  proposerId: number;
  totalPrice: number;
  deliveryFee: number;
  status: string;
  expireAt: string | null;
  createDate: string;
};

export type ProposalItem = {
  id: number;
  itemType: string;
  productId: number | null;
  serviceId: number | null;
  description: string | null;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  displayOrder: number;
};

export type ProposalDetail = {
  id: number;
  requestId: number;
  proposerType: string;
  proposerId: number;
  totalPrice: number;
  deliveryFee: number;
  message: string | null;
  proposedDate: string | null;
  proposedTimeFrom: string | null;
  proposedTimeTo: string | null;
  status: string;
  expireAt: string | null;
  createDate: string;
  updateDate: string | null;
  items: ProposalItem[];
};

export type AcceptProposalResult = {
  proposalId: number;
  status: string;
  dealId: number;
};
