import { Alert, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { useAcceptProposal } from "../hooks/useAcceptProposal";
import { isPendingProposal } from "../model/proposalDisplay";

type AcceptProposalButtonProps = {
  proposalId: number;
  status: string;
  fullWidth?: boolean;
};

export function AcceptProposalButton({
  proposalId,
  status,
  fullWidth = false,
}: AcceptProposalButtonProps) {
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
            <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
              {t("proposal.dealCreatedTitle")}
            </Typography>
            {t("proposal.acceptSuccess", { dealId: accept.data.dealId })}
          </Alert>
          <Button
            component={RouterLink}
            to={`/customer/deals/${accept.data.dealId}`}
            variant="contained"
            size="large"
            sx={{ minHeight: 48, width: fullWidth ? "100%" : { xs: "100%", sm: "auto" } }}
          >
            {t("proposal.viewDeal")}
          </Button>
        </>
      ) : null}
      {isPendingProposal(status) && !accept.isSuccess ? (
        <Button
          variant="contained"
          size="large"
          disabled={accept.isPending}
          onClick={() => {
            accept.mutate(proposalId);
          }}
          sx={{ minHeight: 48, width: fullWidth ? "100%" : { xs: "100%", sm: "auto" } }}
        >
          {accept.isPending ? t("proposal.accepting") : t("proposal.accept")}
        </Button>
      ) : null}
    </Stack>
  );
}
