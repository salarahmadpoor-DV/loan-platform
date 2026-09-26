export type ProviderInvitation = {
  id: number;
  businessName: string | null;
  status: string;
  createdAt: string;
};

export type InvitationActionResult = {
  businessProviderId: number;
  status: string;
};
