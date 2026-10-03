import { Alert, Box, Button, Stack } from "@mui/material";
import { useState } from "react";
import { useLocation } from "react-router-dom";
import { t } from "../../../shared/i18n";
import { AppCard } from "../../../shared/ui/AppCard";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../shared/ui/LoadingState";
import { PageHeader } from "../../../shared/ui/PageHeader";
import type { NotificationItem } from "../api/notificationTypes";
import {
  useMarkAllNotificationsAsRead,
  useMarkNotificationAsRead,
  useMyNotifications,
} from "../hooks/useNotifications";
import { resolveInAppNotificationPath } from "../model/notificationNavigation";
import { NotificationListItem } from "../components/NotificationListItem";
import { useWorkspaceNavigation } from "../../workspace/hooks/useWorkspaceNavigation";
import { workspaceFromPath } from "../../workspace/model/workspacePath";

export function NotificationCenterPage() {
  const { pathname } = useLocation();
  const { openPath } = useWorkspaceNavigation();
  const currentWorkspace = workspaceFromPath(pathname) ?? undefined;
  const [pageSize, setPageSize] = useState(20);
  const [notice, setNotice] = useState<string | null>(null);
  const list = useMyNotifications(1, pageSize);
  const markRead = useMarkNotificationAsRead();
  const markAll = useMarkAllNotificationsAsRead();
  const data = list.data;
  const items = data?.items ?? [];
  const unreadCount = data?.unreadCount ?? 0;
  const canLoadMore = data != null && items.length < data.totalCount && pageSize < 100;

  async function openItem(item: NotificationItem) {
    if (!item.isRead) {
      markRead.mutate(item.id);
    }
    const path = resolveInAppNotificationPath(item.actionUrl);
    if (!path) {
      return;
    }
    const opened = await openPath(path, currentWorkspace);
    if (!opened) {
      setNotice(t("notifications.workspaceUnavailable"));
    }
  }

  return (
    <>
      <PageHeader
        title={t("nav.notifications")}
        description={t("notifications.description")}
        action={
          unreadCount > 0 ? (
            <Button variant="contained" disabled={markAll.isPending} onClick={() => markAll.mutate()}>
              {t("notifications.markAllRead")}
            </Button>
          ) : undefined
        }
      />
      {notice ? <Alert severity="info" sx={{ mb: 2 }}>{notice}</Alert> : null}
      {list.isPending ? <LoadingState /> : null}
      {list.isError ? <ErrorAlert error={list.error} /> : null}
      {markAll.isError ? <ErrorAlert error={markAll.error} /> : null}
      {markRead.isError ? <ErrorAlert error={markRead.error} /> : null}
      {!list.isPending && !list.isError && items.length === 0 ? (
        <EmptyState title={t("notifications.emptyTitle")} body={t("notifications.emptyBody")} />
      ) : null}
      <Stack spacing={2}>
        {items.map((item) => (
          <AppCard key={item.id}>
            <NotificationListItem
              item={item}
              onOpen={openItem}
              onMarkRead={(current) => markRead.mutate(current.id)}
              markingId={markRead.isPending ? markRead.variables : undefined}
            />
          </AppCard>
        ))}
      </Stack>
      {canLoadMore ? (
        <Box sx={{ mt: 2 }}>
          <Button onClick={() => setPageSize((current) => Math.min(100, current + 20))}>
            {t("notifications.loadMore")}
          </Button>
        </Box>
      ) : null}
    </>
  );
}
