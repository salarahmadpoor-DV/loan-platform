import { StatusChip } from "../../../../shared/ui/StatusChip";
import { dealStatusLabel, dealStatusTone } from "../model/dealDisplay";

type DealStatusChipProps = {
  status: string;
};

export function DealStatusChip({ status }: DealStatusChipProps) {
  return <StatusChip label={dealStatusLabel(status)} tone={dealStatusTone(status)} />;
}
