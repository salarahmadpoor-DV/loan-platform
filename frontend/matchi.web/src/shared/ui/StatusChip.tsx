import { Chip } from "@mui/material";

type StatusChipProps = {
  label: string;
};

export function StatusChip({ label }: StatusChipProps) {
  return <Chip size="small" label={label} color="default" />;
}
