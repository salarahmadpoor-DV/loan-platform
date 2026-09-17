/** Live GET /api/providers/me/businesses (`ProviderMembershipDto`). */
export type ProviderBusinessMembership = {
  businessId: number;
  businessName: string | null;
  role: string;
  status: string;
  joinedAt: string;
  leftAt: string | null;
};
