import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { QueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../shared/api/queryKeys";
import type { NotificationList } from "../api/notificationTypes";
import {
  getMyNotifications,
  getUnreadNotificationCount,
  markAllNotificationsAsRead,
  markNotificationAsRead,
} from "../api/notificationsApi";

const notificationListPrefix = [...queryKeys.notifications.all, "list"] as const;

function isNotificationList(data: unknown): data is NotificationList {
  return typeof data === "object" && data !== null && Array.isArray((data as NotificationList).items);
}

function patchUnreadCount(queryClient: QueryClient, unreadCount: number) {
  queryClient.setQueryData(queryKeys.notifications.unread(), { unreadCount });
}

export function useUnreadNotificationCount(enabled = true) {
  return useQuery({
    queryKey: queryKeys.notifications.unread(),
    queryFn: getUnreadNotificationCount,
    enabled,
    refetchInterval: enabled ? 30_000 : false,
  });
}

export function useMyNotifications(page: number, pageSize: number, enabled = true) {
  return useQuery({
    queryKey: queryKeys.notifications.list(page, pageSize),
    queryFn: () => getMyNotifications(page, pageSize),
    enabled,
  });
}

export function useMarkNotificationAsRead() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (notificationId: number) => markNotificationAsRead(notificationId),
    onSuccess: (_, notificationId) => {
      let markedUnread = false;
      queryClient.setQueriesData({ queryKey: notificationListPrefix }, (current) => {
        if (!isNotificationList(current)) {
          return current;
        }
        let changed = false;
        const items = current.items.map((item) => {
          if (item.id !== notificationId || item.isRead) {
            return item;
          }
          changed = true;
          markedUnread = true;
          return { ...item, isRead: true, readAt: new Date().toISOString() };
        });
        return {
          ...current,
          items,
          unreadCount: Math.max(0, current.unreadCount - (changed ? 1 : 0)),
        };
      });
      if (markedUnread) {
        const unread = queryClient.getQueryData<{ unreadCount: number }>(queryKeys.notifications.unread());
        const currentCount = unread?.unreadCount;
        patchUnreadCount(
          queryClient,
          Math.max(0, (typeof currentCount === "number" ? currentCount : 1) - 1),
        );
      }
    },
  });
}

export function useMarkAllNotificationsAsRead() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: markAllNotificationsAsRead,
    onSuccess: () => {
      queryClient.setQueriesData({ queryKey: notificationListPrefix }, (current) => {
        if (!isNotificationList(current)) {
          return current;
        }
        return {
          ...current,
          unreadCount: 0,
          items: current.items.map((item) =>
            item.isRead ? item : { ...item, isRead: true, readAt: new Date().toISOString() },
          ),
        };
      });
      patchUnreadCount(queryClient, 0);
    },
  });
}
