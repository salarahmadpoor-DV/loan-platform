import { Alert, Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { useRejectProposal } from "../hooks/useRejectProposal";
import { isPendingProposal } from "../model/proposalDisplay";

type RejectProposalButtonProps = {
  proposalId: number;
  status: string;
  fullWidth?: boolean;
};

export function RejectProposalButton({
  proposalId,
  status,
  fullWidth = false,
}: RejectProposalButtonProps) {
  const reject = useRejectProposal();

  if (!isPendingProposal(status) && !reject.isSuccess) {
    return null;
  }

  return (
    <Stack spacing={1.5}>
      {reject.isError ? <ErrorAlert error={reject.error} /> : null}
      {reject.isSuccess ? (
        <Alert severity="info">
          <Typography variant="subtitle2">{t("proposal.rejectSuccess")}</Typography>
        </Alert>
      ) : null}
      {isPendingProposal(status) && !reject.isSuccess ? (
        <Button
          variant="outlined"
          color="error"
          size="large"
          disabled={reject.isPending}
          onClick={() => {
            reject.mutate(proposalId);
          }}
          sx={{ minHeight: 48, width: fullWidth ? "100%" : { xs: "100%", sm: "auto" } }}
        >
          {reject.isPending ? t("proposal.rejecting") : t("proposal.reject")}
        </Button>
      ) : null}
    </Stack>
  );
}
