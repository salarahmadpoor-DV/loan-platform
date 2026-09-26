export type LocationProvince = {
  id: number;
  name: string;
  code: string;
};

export type LocationCity = {
  id: number;
  provinceId: number;
  name: string;
};

export type LocationDistrict = {
  id: number;
  cityId: number;
  name: string;
};

export type LocationSelection = {
  provinceId: number | null;
  cityId: number | null;
  districtId: number | null;
  provinceName: string | null;
  cityName: string | null;
  districtName: string | null;
};

export type LocationResolveResult = {
  provinceId: number;
  provinceName: string;
  cityId: number;
  cityName: string;
  districtId: number | null;
  districtName: string | null;
  source: "External" | "Internal" | string;
};

export type LocationResolveResponse = {
  success: boolean;
  data: LocationResolveResult | null;
};

export const emptyLocationSelection: LocationSelection = {
  provinceId: null,
  cityId: null,
  districtId: null,
  provinceName: null,
  cityName: null,
  districtName: null,
};
