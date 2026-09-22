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

/** Live GET /api/providers/me/areas (`ProviderServiceAreaDto`). */
export type ProviderServiceArea = {
  id: number;
  areaType: string;
  province: string | null;
  city: string | null;
  district: string | null;
  lat: number | null;
  lng: number | null;
  radius: number | null;
  isActive: boolean;
};
