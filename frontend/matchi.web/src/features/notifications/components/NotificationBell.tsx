import { Alert, Button, Divider, Popover, Stack, Typography } from "@mui/material";
import Badge from "@mui/material/Badge";
import IconButton from "@mui/material/IconButton";
import { useTheme } from "@mui/material/styles";
import { useState, type MouseEvent } from "react";
import { useNavigate } from "react-router-dom";
import { t } from "../../../shared/i18n";
import { chromeIconFontSize, NotificationsNoneOutlined } from "../../../shared/ui/icons";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../shared/ui/LoadingState";
import type { AppWorkspace } from "../../../shared/navigation/navModel";
import { workspaceNotificationsPath } from "../../../shared/navigation/navModel";
import type { NotificationItem } from "../api/notificationTypes";
import {
  useMarkAllNotificationsAsRead,
  useMarkNotificationAsRead,
  useMyNotifications,
  useUnreadNotificationCount,
} from "../hooks/useNotifications";
import { resolveInAppNotificationPath } from "../model/notificationNavigation";
import { NotificationListItem } from "./NotificationListItem";
import { useWorkspaceNavigation } from "../../workspace/hooks/useWorkspaceNavigation";

type NotificationBellProps = {
  workspace: AppWorkspace;
};

export function NotificationBell({ workspace }: NotificationBellProps) {
  const theme = useTheme();
  const navigate = useNavigate();
  const { openPath } = useWorkspaceNavigation();
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const open = Boolean(anchorEl);
  const unread = useUnreadNotificationCount();
  const list = useMyNotifications(1, 8, open);
  const markRead = useMarkNotificationAsRead();
  const markAll = useMarkAllNotificationsAsRead();
  const unreadCount = unread.data?.unreadCount ?? 0;
  const items = list.data?.items ?? [];
  const centerPath = workspaceNotificationsPath(workspace);
  const endAlign = theme.direction === "rtl" ? "left" : "right";

  function close() {
    setAnchorEl(null);
  }

  async function openItem(item: NotificationItem) {
    if (!item.isRead) {
      markRead.mutate(item.id);
    }
    const path = resolveInAppNotificationPath(item.actionUrl);
    close();
    if (!path) {
      return;
    }
    const opened = await openPath(path, workspace);
    if (!opened) {
      setNotice(t("notifications.workspaceUnavailable"));
    }
  }

  return (
    <>
      <IconButton
        color="inherit"
        aria-label={t("nav.notifications")}
        aria-haspopup="dialog"
        aria-expanded={open}
        onClick={(event: MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget)}
        sx={{ minWidth: 44, minHeight: 44 }}
      >
        <Badge
          badgeContent={unreadCount}
          color="primary"
          max={99}
          overlap="circular"
          invisible={unreadCount < 1}
          sx={{ "& .MuiBadge-badge": { fontSize: 10, height: 16, minWidth: 16, px: 0.5 } }}
        >
          <NotificationsNoneOutlined aria-hidden sx={{ fontSize: chromeIconFontSize }} />
        </Badge>
      </IconButton>
      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={close}
        anchorOrigin={{ vertical: "bottom", horizontal: endAlign }}
        transformOrigin={{ vertical: "top", horizontal: endAlign }}
        slotProps={{
          paper: {
            sx: {
              width: 360,
              maxWidth: "calc(100vw - 16px)",
              maxHeight: "min(70vh, 520px)",
              overflowY: "auto",
              mt: 0.5,
            },
          },
        }}
      >
        <Stack spacing={1.5} sx={{ p: 1.5 }}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={1}>
            <Typography variant="subtitle1" fontWeight={700}>
              {t("nav.notifications")}
            </Typography>
            {unreadCount > 0 ? (
              <Button
                size="small"
                variant="contained"
                disabled={markAll.isPending}
                onClick={() => markAll.mutate()}
              >
                {t("notifications.markAllRead")}
              </Button>
            ) : null}
          </Stack>
          {notice ? <Alert severity="info">{notice}</Alert> : null}
          {list.isPending ? <LoadingState /> : null}
          {list.isError ? <ErrorAlert error={list.error} /> : null}
          {markAll.isError ? <ErrorAlert error={markAll.error} /> : null}
          {!list.isPending && !list.isError && items.length === 0 ? (
            <EmptyState title={t("notifications.emptyTitle")} body={t("notifications.emptyBody")} />
          ) : null}
          <Stack divider={<Divider />} sx={{ mx: -1.5 }}>
            {items.map((item) => (
              <NotificationListItem key={item.id} item={item} compact onOpen={openItem} />
            ))}
          </Stack>
          <Button
            onClick={() => {
              close();
              navigate(centerPath);
            }}
          >
            {t("notifications.viewAll")}
          </Button>
        </Stack>
      </Popover>
    </>
  );
}
