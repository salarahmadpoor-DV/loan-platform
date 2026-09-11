/** Live GET /api/provider/requests (`ProviderRequestInboxItemDto`). */
export type ProviderInboxLocation = {
  province: string | null;
  city: string | null;
  district: string | null;
};

export type ProviderRequestInboxItem = {
  requestId: number;
  requestType: string;
  serviceSummary: string | null;
  categorySummary: string | null;
  location: ProviderInboxLocation | null;
  createdDate: string;
  status: string;
};
