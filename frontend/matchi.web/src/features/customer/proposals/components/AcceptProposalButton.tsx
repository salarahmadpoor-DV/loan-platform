import { Alert, Button, Stack } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { useAcceptProposal } from "../hooks/useAcceptProposal";
import { isPendingProposal } from "../model/proposalDisplay";

type AcceptProposalButtonProps = {
  proposalId: number;
  status: string;
};

export function AcceptProposalButton({ proposalId, status }: AcceptProposalButtonProps) {
  const accept = useAcceptProposal();

  if (!isPendingProposal(status) && !accept.isSuccess) {
    return null;
  }

  return (
    <Stack spacing={1.5}>
      {accept.isError ? <ErrorAlert error={accept.error} /> : null}
      {accept.isSuccess ? (
        <>
          <Alert severity="success">
            {t("proposal.acceptSuccess", { dealId: accept.data.dealId })}
          </Alert>
          <Button
            component={RouterLink}
            to={`/customer/deals/${accept.data.dealId}`}
            variant="contained"
            sx={{ alignSelf: "flex-start" }}
          >
            {t("proposal.viewDeal")}
          </Button>
        </>
      ) : null}
      {isPendingProposal(status) && !accept.isSuccess ? (
        <Button
          variant="contained"
          disabled={accept.isPending}
          onClick={() => {
            accept.mutate(proposalId);
          }}
          sx={{ alignSelf: "flex-start" }}
        >
          {accept.isPending ? t("proposal.accepting") : t("proposal.accept")}
        </Button>
      ) : null}
    </Stack>
  );
}
