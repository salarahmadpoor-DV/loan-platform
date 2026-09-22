import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyProviderInvitations } from "../api/invitationsApi";

export function useMyProviderInvitations() {
  return useQuery({
    queryKey: queryKeys.provider.invitations(),
    queryFn: getMyProviderInvitations,
  });
}
