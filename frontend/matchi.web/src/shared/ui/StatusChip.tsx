import { Chip, type ChipProps } from "@mui/material";

export type StatusTone = "neutral" | "pending" | "success" | "danger" | "info" | "primary";

type StatusChipProps = {
  label: string;
  tone?: StatusTone;
};

const TONE_COLOR: Record<StatusTone, ChipProps["color"]> = {
  neutral: "default",
  pending: "warning",
  success: "success",
  danger: "error",
  info: "info",
  primary: "primary",
};

export function StatusChip({ label, tone = "neutral" }: StatusChipProps) {
  return (
    <Chip
      size="small"
      label={label}
      color={TONE_COLOR[tone]}
      sx={{ fontWeight: 600 }}
    />
  );
}
