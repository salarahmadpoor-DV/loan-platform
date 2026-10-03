export type NotificationItem = {
  id: number;
  type: string;
  title: string;
  message: string;
  entityType: string | null;
  entityId: number | null;
  actionUrl: string | null;
  isRead: boolean;
  createdAt: string;
  readAt: string | null;
};

export type NotificationList = {
  items: NotificationItem[];
  page: number;
  pageSize: number;
  totalCount: number;
  unreadCount: number;
};

export type UnreadNotificationCount = {
  unreadCount: number;
};
