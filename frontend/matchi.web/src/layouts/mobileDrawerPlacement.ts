/**
 * stylis-plugin-rtl flips CSS left/right. MUI Drawer `anchor` also sets left/right.
 * Passing `right` in RTL therefore lands on the left. Pin the paper to a physical
 * edge with !important so the drawer matches document direction.
 */
export function mobileDrawerAnchor(direction: string): "left" | "right" {
  return direction === "rtl" ? "right" : "left";
}

type DrawerWidth = string | number | { xs?: string | number; sm?: string | number };

export function mobileDrawerPaperSx(direction: string, width: DrawerWidth) {
  const rtl = direction === "rtl";
  return {
    width,
    boxSizing: "border-box" as const,
    direction: rtl ? ("rtl" as const) : ("ltr" as const),
    ...(rtl
      ? { left: "auto !important", right: "0 !important" }
      : { right: "auto !important", left: "0 !important" }),
  };
}
