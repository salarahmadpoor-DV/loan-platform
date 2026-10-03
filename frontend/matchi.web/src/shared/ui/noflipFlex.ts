import type { CSSProperties } from "react";
import { css } from "@emotion/react";

/**
 * stylis-plugin-rtl converts `flex-direction: row` to `row-reverse`.
 * Combined with `dir="rtl"` that double-reverses and looks LTR.
 */
export const noflipFlexRow = css`
  /* @noflip */
  flex-direction: row;
`;

export const noflipChromeGrid = css`
  /* @noflip */
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto;
`;

/** Inline styles skip Emotion RTL rewriting. Use with `dir` on the same node. */
export const rtlSafeFlexRow: CSSProperties = {
  display: "flex",
  flexDirection: "row",
  alignItems: "center",
};
