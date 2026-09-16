import { Box } from "@mui/material";
import { matchiRadius } from "../../../../app/designTokens";

type HeroVisualProps = {
  label: string;
};

export function HeroVisual({ label }: HeroVisualProps) {
  return (
    <Box
      aria-hidden={false}
      role="img"
      aria-label={label}
      sx={{
        position: "relative",
        minHeight: { xs: 200, md: 320 },
        borderRadius: matchiRadius.lg,
        bgcolor: "primary.main",
        overflow: "hidden",
      }}
    >
      <Box
        sx={{
          position: "absolute",
          width: "55%",
          height: "55%",
          borderRadius: "50%",
          bgcolor: "secondary.main",
          opacity: 0.85,
          insetInlineEnd: "-8%",
          top: "-12%",
        }}
      />
      <Box
        sx={{
          position: "absolute",
          width: "40%",
          height: "40%",
          borderRadius: "50%",
          bgcolor: "background.paper",
          opacity: 0.2,
          insetInlineStart: "8%",
          bottom: "10%",
        }}
      />
      <Box
        sx={{
          position: "absolute",
          inset: { xs: 28, md: 40 },
          borderRadius: matchiRadius.md,
          bgcolor: "background.paper",
          opacity: 0.16,
        }}
      />
    </Box>
  );
}
