/** Live GET /api/businesses/me (`BusinessProfileDto`). */
export type MyBusinessProfile = {
  id: number;
  ownerUserId: number;
  name: string;
  description: string | null;
  mobile: string | null;
  address: string | null;
  province: string | null;
  city: string | null;
  district: string | null;
  lat: number | null;
  lng: number | null;
  logoMediaId: number | null;
  rating: number;
  reviewCount: number;
  completedJobCount: number;
  status: string;
};
