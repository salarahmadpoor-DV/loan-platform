import { StatusChip } from "../../../../shared/ui/StatusChip";
import { proposalStatusLabel } from "../model/proposalDisplay";

type ProposalStatusChipProps = {
  status: string;
};

export function ProposalStatusChip({ status }: ProposalStatusChipProps) {
  return <StatusChip label={proposalStatusLabel(status)} />;
}
