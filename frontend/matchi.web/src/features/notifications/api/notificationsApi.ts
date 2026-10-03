import { getJson, postJson } from "../../../shared/api/httpClient";
import type { NotificationList, UnreadNotificationCount } from "./notificationTypes";

export function getMyNotifications(page = 1, pageSize = 20): Promise<NotificationList> {
  return getJson<NotificationList>("/api/notifications", {
    params: { page, pageSize },
  });
}

export function getUnreadNotificationCount(): Promise<UnreadNotificationCount> {
  return getJson<UnreadNotificationCount>("/api/notifications/unread-count");
}

export function markNotificationAsRead(notificationId: number): Promise<void> {
  return postJson(`/api/notifications/${notificationId}/read`);
}

export function markAllNotificationsAsRead(): Promise<{ updated: number }> {
  return postJson<{ updated: number }>("/api/notifications/read-all");
}
