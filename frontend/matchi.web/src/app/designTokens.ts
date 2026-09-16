/**
 * Matchi visual tokens. `app/theme.ts` maps these onto MUI.
 * Changing `primary` / `secondary` here (and rebuilding the theme) changes the product identity.
 */
export const matchiColors = {
  primary: "#2563EB",
  primaryDark: "#1D4ED8",
  primaryLight: "#60A5FA",
  secondary: "#14B8A6",
  secondaryDark: "#0F766E",
  secondaryLight: "#5EEAD4",
  background: "#F8FAFC",
  surface: "#FFFFFF",
  text: "#0F172A",
  mutedText: "#64748B",
  border: "#E2E8F0",
  success: "#059669",
  warning: "#D97706",
  error: "#DC2626",
  contrastText: "#FFFFFF",
} as const;

export const matchiRadius = {
  sm: 8,
  md: 12,
  lg: 16,
} as const;

export const matchiShadows = {
  card: "0 1px 2px rgba(15, 23, 42, 0.06)",
  elevated: "0 12px 32px rgba(15, 23, 42, 0.08)",
} as const;
