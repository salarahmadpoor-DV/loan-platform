import { Box, Button, Stack, Typography } from "@mui/material";
import { formatDateTime } from "../../customer/proposals/model/proposalDisplay";
import { t } from "../../../shared/i18n";
import { StatusChip } from "../../../shared/ui/StatusChip";
import type { NotificationItem } from "../api/notificationTypes";

type NotificationListItemProps = {
  item: NotificationItem;
  compact?: boolean;
  onOpen: (item: NotificationItem) => void;
  onMarkRead?: (item: NotificationItem) => void;
  markingId?: number;
};

export function NotificationListItem({
  item,
  compact = false,
  onOpen,
  onMarkRead,
  markingId,
}: NotificationListItemProps) {
  return (
    <Box
      sx={{
        px: compact ? 1.5 : 0,
        py: compact ? 1.25 : 0,
        bgcolor: item.isRead ? "transparent" : "action.hover",
      }}
    >
      <Stack spacing={0.75}>
        <Box
          role="button"
          tabIndex={0}
          onClick={() => onOpen(item)}
          onKeyDown={(event) => {
            if (event.key === "Enter" || event.key === " ") {
              event.preventDefault();
              onOpen(item);
            }
          }}
          sx={{ cursor: "pointer" }}
        >
          <Stack direction="row" spacing={1} alignItems="flex-start" justifyContent="space-between">
            <Typography variant={compact ? "subtitle2" : "subtitle1"} fontWeight={item.isRead ? 500 : 700}>
              {item.title}
            </Typography>
            {!compact ? (
              <StatusChip
                label={item.isRead ? t("notifications.read") : t("notifications.unread")}
                tone={item.isRead ? "neutral" : "info"}
              />
            ) : null}
          </Stack>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{
              mt: 0.5,
              display: "-webkit-box",
              WebkitLineClamp: compact ? 2 : 4,
              WebkitBoxOrient: "vertical",
              overflow: "hidden",
            }}
          >
            {item.message}
          </Typography>
          <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.5 }}>
            {formatDateTime(item.createdAt)}
          </Typography>
        </Box>
        {!item.isRead && onMarkRead && !compact ? (
          <Box>
            <Button size="small" disabled={markingId === item.id} onClick={() => onMarkRead(item)}>
              {t("notifications.markRead")}
            </Button>
          </Box>
        ) : null}
      </Stack>
    </Box>
  );
}
