import { getJson, putJson } from "../../../../shared/api/httpClient";
import type { InvitationActionResult, ProviderInvitation } from "./invitationTypes";

export function getMyProviderInvitations(): Promise<ProviderInvitation[]> {
  return getJson<ProviderInvitation[]>("/api/providers/me/invitations");
}

export function acceptProviderInvitation(businessProviderId: number): Promise<InvitationActionResult> {
  return putJson<InvitationActionResult>(
    `/api/businesses/business-providers/${businessProviderId}/accept`,
  );
}

export function rejectProviderInvitation(businessProviderId: number): Promise<InvitationActionResult> {
  return putJson<InvitationActionResult>(
    `/api/businesses/business-providers/${businessProviderId}/reject`,
  );
}
