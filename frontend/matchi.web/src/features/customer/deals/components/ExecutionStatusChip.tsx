import { StatusChip, type StatusTone } from "../../../../shared/ui/StatusChip";
import { executionStatusLabel } from "../model/dealDisplay";

type ExecutionStatusChipProps = {
  status: string;
};

function executionTone(status: string): StatusTone {
  const key = status.toLowerCase();
  if (key === "pending") {
    return "pending";
  }
  if (key === "inprogress") {
    return "info";
  }
  if (key === "completed") {
    return "success";
  }
  if (key === "cancelled") {
    return "neutral";
  }
  return "neutral";
}

export function ExecutionStatusChip({ status }: ExecutionStatusChipProps) {
  return <StatusChip label={executionStatusLabel(status)} tone={executionTone(status)} />;
}
