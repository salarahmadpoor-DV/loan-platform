import { StatusChip } from "../../../../shared/ui/StatusChip";
import { executionStatusLabel } from "../model/dealDisplay";

type ExecutionStatusChipProps = {
  status: string;
};

export function ExecutionStatusChip({ status }: ExecutionStatusChipProps) {
  return <StatusChip label={executionStatusLabel(status)} />;
}
