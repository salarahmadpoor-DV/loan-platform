import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../../shared/api/queryKeys";
import { createRequest } from "../api/createRequestApi";
import type { CreateRequestBody } from "../api/createRequestTypes";

export function useCreateRequest() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateRequestBody) => createRequest(body),
    onSuccess: (result) => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.requests.all });
      void queryClient.invalidateQueries({
        queryKey: queryKeys.requests.detail(result.requestId),
      });
    },
  });
}
