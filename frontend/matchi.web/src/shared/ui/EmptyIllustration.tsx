import { Box } from "@mui/material";

export type EmptyIllustrationKind = "requests" | "deals" | "reviews" | "search";

type EmptyIllustrationProps = {
  kind: EmptyIllustrationKind;
};

/**
 * Compact inline SVG (no network). Decorative; parent EmptyState stays text-first.
 */
export function EmptyIllustration({ kind }: EmptyIllustrationProps) {
  return (
    <Box
      component="svg"
      viewBox="0 0 120 72"
      aria-hidden
      sx={{
        width: 120,
        maxWidth: "42%",
        height: "auto",
        display: "block",
        color: "primary.main",
        mb: 1.5,
      }}
    >
      <rect x="8" y="14" width="104" height="50" rx="10" fill="currentColor" opacity="0.08" />
      {kind === "requests" ? (
        <>
          <rect x="24" y="26" width="48" height="6" rx="3" fill="currentColor" opacity="0.45" />
          <rect x="24" y="38" width="72" height="4" rx="2" fill="currentColor" opacity="0.28" />
          <rect x="24" y="48" width="56" height="4" rx="2" fill="currentColor" opacity="0.28" />
        </>
      ) : null}
      {kind === "deals" ? (
        <>
          <circle cx="44" cy="40" r="12" fill="none" stroke="currentColor" strokeWidth="3" opacity="0.45" />
          <circle cx="76" cy="40" r="12" fill="none" stroke="currentColor" strokeWidth="3" opacity="0.45" />
        </>
      ) : null}
      {kind === "reviews" ? (
        <>
          <path
            d="M28 48 V28 h48 a8 8 0 0 1 8 8 v8 H52 l-8 12 z"
            fill="currentColor"
            opacity="0.35"
          />
          <rect x="40" y="34" width="24" height="3" rx="1.5" fill="#fff" opacity="0.9" />
        </>
      ) : null}
      {kind === "search" ? (
        <>
          <circle cx="52" cy="36" r="14" fill="none" stroke="currentColor" strokeWidth="3" opacity="0.45" />
          <line x1="62" y1="48" x2="82" y2="62" stroke="currentColor" strokeWidth="3" strokeLinecap="round" opacity="0.45" />
        </>
      ) : null}
    </Box>
  );
}
