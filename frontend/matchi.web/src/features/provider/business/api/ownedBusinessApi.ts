import { deleteJson, getJson, postJson, putJson } from "../../../../shared/api/httpClient";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";

export type BusinessMembership = {
  providerId: number;
  providerName: string | null;
  role: string;
  status: string;
  joinedAt: string;
  leftAt: string | null;
};

export type CreateOwnedBusinessBody = {
  name: string;
  description: string | null;
  mobile: string | null;
  address: string | null;
  province: string | null;
  city: string | null;
  district: string | null;
};

export type CreateOwnedBusinessResult = {
  businessId: number;
};

export function createOwnedBusiness(body: CreateOwnedBusinessBody): Promise<CreateOwnedBusinessResult> {
  return postJson<CreateOwnedBusinessResult, CreateOwnedBusinessBody>("/api/businesses", body);
}

export type UpdateOwnedBusinessBody = {
  businessId?: number;
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
};

export type InviteProviderBody = {
  providerId?: number;
  mobile?: string;
  role?: string;
};

export type InviteProviderResult = {
  inviteId: number;
  status: string;
};

export function getOwnedBusinessTeam(businessId?: number): Promise<BusinessMembership[]> {
  const query = businessId != null ? `?businessId=${businessId}` : "";
  return getJson<BusinessMembership[]>(`/api/businesses/me/providers${query}`);
}

export function updateOwnedBusiness(body: UpdateOwnedBusinessBody): Promise<{ success: boolean }> {
  return putJson<{ success: boolean }, UpdateOwnedBusinessBody>("/api/businesses/me", body);
}

export function inviteBusinessProvider(
  businessId: number,
  body: InviteProviderBody,
): Promise<InviteProviderResult> {
  return postJson<InviteProviderResult, InviteProviderBody>(
    `/api/businesses/${businessId}/invite-provider`,
    body,
  );
}

export function removeBusinessProvider(providerId: number, businessId?: number): Promise<void> {
  const query = businessId != null ? `?businessId=${businessId}` : "";
  return deleteJson(`/api/businesses/me/providers/${providerId}${query}`);
}

export type { MyBusinessProfile };
