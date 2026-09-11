import { StatusChip } from "../../../../shared/ui/StatusChip";

type RequestStatusChipProps = {
  status: string;
};

export function RequestStatusChip({ status }: RequestStatusChipProps) {
  return <StatusChip label={status} />;
}
