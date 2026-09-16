import { useProviderRequestInbox } from "./useProviderRequestInbox";

export function useProviderInboxItem(requestId: number | undefined) {
  const inbox = useProviderRequestInbox();
  const item =
    requestId != null
      ? inbox.data?.find((entry) => entry.requestId === requestId)
      : undefined;

  return {
    ...inbox,
    item,
    notFound: inbox.isSuccess && requestId != null && !item,
  };
}
