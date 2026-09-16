import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestStatusLabel, requestStatusTone } from "../model/requestPresentation";

type RequestStatusChipProps = {
  status: string;
};

export function RequestStatusChip({ status }: RequestStatusChipProps) {
  return <StatusChip label={requestStatusLabel(status)} tone={requestStatusTone(status)} />;
}
