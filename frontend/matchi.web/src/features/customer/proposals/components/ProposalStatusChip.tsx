import { StatusChip } from "../../../../shared/ui/StatusChip";
import { proposalStatusLabel, proposalStatusTone } from "../model/proposalDisplay";

type ProposalStatusChipProps = {
  status: string;
};

export function ProposalStatusChip({ status }: ProposalStatusChipProps) {
  return <StatusChip label={proposalStatusLabel(status)} tone={proposalStatusTone(status)} />;
}
