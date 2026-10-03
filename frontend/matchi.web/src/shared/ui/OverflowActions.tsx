import { Box, Divider, IconButton, Menu, MenuItem } from "@mui/material";
import { useState, type MouseEvent, type ReactNode } from "react";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../i18n";
import { MoreHoriz } from "./icons";

export type OverflowActionItem = {
  key: string;
  label: string;
  to?: string;
  onClick?: () => void;
  destructive?: boolean;
  disabled?: boolean;
};

type OverflowActionsProps = {
  items: OverflowActionItem[];
  label?: string;
};

export function OverflowActions({ items, label }: OverflowActionsProps) {
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);
  if (items.length === 0) {
    return null;
  }

  function close() {
    setAnchor(null);
  }

  const firstDestructive = items.findIndex((item) => item.destructive);

  return (
    <>
      <IconButton
        size="small"
        aria-label={label ?? t("action.more")}
        aria-haspopup="menu"
        aria-expanded={Boolean(anchor)}
        onClick={(event: MouseEvent<HTMLElement>) => setAnchor(event.currentTarget)}
        sx={{ minWidth: 44, minHeight: 44, flexShrink: 0 }}
      >
        <MoreHoriz aria-hidden />
      </IconButton>
      <Menu anchorEl={anchor} open={Boolean(anchor)} onClose={close}>
        {items.map((item, index) => (
          <Box key={item.key}>
            {firstDestructive > 0 && index === firstDestructive ? <Divider /> : null}
            {item.to ? (
              <MenuItem
                component={RouterLink}
                to={item.to}
                disabled={item.disabled}
                onClick={() => {
                  item.onClick?.();
                  close();
                }}
                sx={{
                  color: item.destructive ? "error.main" : undefined,
                  minHeight: 44,
                }}
              >
                {item.label}
              </MenuItem>
            ) : (
              <MenuItem
                disabled={item.disabled}
                onClick={() => {
                  item.onClick?.();
                  close();
                }}
                sx={{
                  color: item.destructive ? "error.main" : undefined,
                  minHeight: 44,
                }}
              >
                {item.label}
              </MenuItem>
            )}
          </Box>
        ))}
      </Menu>
    </>
  );
}

type ItemActionsProps = {
  primary: ReactNode;
  items?: OverflowActionItem[];
};

/** One visible primary action; secondary actions go in "...". Stacks on xs. */
export function ItemActions({ primary, items = [] }: ItemActionsProps) {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: { xs: "column", sm: "row" },
        alignItems: { xs: "stretch", sm: "center" },
        justifyContent: "space-between",
        gap: 1,
        mt: "auto",
        pt: 0.5,
      }}
    >
      <Box sx={{ minWidth: 0, "& > *": { width: { xs: "100%", sm: "auto" } } }}>{primary}</Box>
      {items.length > 0 ? (
        <Box sx={{ alignSelf: { xs: "flex-end", sm: "center" } }}>
          <OverflowActions items={items} />
        </Box>
      ) : null}
    </Box>
  );
}
