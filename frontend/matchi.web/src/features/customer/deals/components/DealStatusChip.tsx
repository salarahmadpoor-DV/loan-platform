import { StatusChip } from "../../../../shared/ui/StatusChip";
import { dealStatusLabel } from "../model/dealDisplay";

type DealStatusChipProps = {
  status: string;
};

export function DealStatusChip({ status }: DealStatusChipProps) {
  return <StatusChip label={dealStatusLabel(status)} />;
}
