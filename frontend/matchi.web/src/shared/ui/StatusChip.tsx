import { Chip, type ChipProps } from "@mui/material";

export type StatusTone = "neutral" | "pending" | "success" | "danger" | "info" | "primary" | "inProgress";

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
  inProgress: "secondary",
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
