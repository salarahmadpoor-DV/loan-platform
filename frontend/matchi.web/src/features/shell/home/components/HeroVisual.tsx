import { Box } from "@mui/material";
import { matchiRadius } from "../../../../app/designTokens";

type HeroVisualProps = {
  label: string;
};

export function HeroVisual({ label }: HeroVisualProps) {
  return (
    <Box
      role="img"
      aria-label={label}
      sx={{
        position: "relative",
        display: { xs: "none", md: "block" },
        minHeight: { md: 340 },
        borderRadius: matchiRadius.lg,
        bgcolor: "primary.main",
        overflow: "hidden",
      }}
    >
      <Box
        sx={{
          position: "absolute",
          width: "58%",
          height: "58%",
          borderRadius: "50%",
          bgcolor: "secondary.main",
          insetInlineEnd: "-10%",
          top: "-14%",
        }}
      />
      <Box
        sx={{
          position: "absolute",
          width: 88,
          height: 88,
          borderRadius: "50%",
          bgcolor: "background.paper",
          opacity: 0.22,
          insetInlineStart: "14%",
          top: "22%",
        }}
      />
      <Box
        sx={{
          position: "absolute",
          width: "42%",
          height: "36%",
          borderRadius: matchiRadius.md,
          bgcolor: "background.paper",
          opacity: 0.18,
          insetInlineStart: "12%",
          bottom: "14%",
        }}
      />
    </Box>
  );
}
