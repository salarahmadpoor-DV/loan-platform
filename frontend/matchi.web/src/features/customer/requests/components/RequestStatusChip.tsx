import { StatusChip } from "../../../../shared/ui/StatusChip";

type RequestStatusChipProps = {
  status: string;
};

export function RequestStatusChip({ status }: RequestStatusChipProps) {
  const key = status.toLowerCase();
  const tone = key === "open" ? "info" : key === "cancelled" ? "neutral" : "pending";
  return <StatusChip label={status} tone={tone} />;
}
