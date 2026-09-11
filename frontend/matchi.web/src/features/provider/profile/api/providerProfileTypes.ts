/** Live GET /api/providers/me (`ProviderProfileDto`). */
export type ProviderProfile = {
  id: number;
  userId: number;
  name: string;
  mobile: string;
  description: string | null;
  lat: number | null;
  lng: number | null;
  rating: number;
  reviewCount: number;
  completedJobCount: number;
  status: string;
};
