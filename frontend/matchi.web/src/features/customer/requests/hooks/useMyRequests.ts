import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getMyRequests } from "../api/requestsApi";

export function useMyRequests() {
  return useQuery({
    queryKey: queryKeys.requests.mine(),
    queryFn: getMyRequests,
  });
}
