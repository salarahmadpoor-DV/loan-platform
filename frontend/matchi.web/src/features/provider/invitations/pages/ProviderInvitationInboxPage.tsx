import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { useAcceptProviderInvitation, useRejectProviderInvitation } from "../hooks/useInvitationActions";
import { useMyProviderInvitations } from "../hooks/useMyProviderInvitations";

export function ProviderInvitationInboxPage() {
  const invitations = useMyProviderInvitations();
  const accept = useAcceptProviderInvitation();
  const reject = useRejectProviderInvitation();
  const items = invitations.data ?? [];
  const busyId = accept.isPending ? accept.variables : reject.isPending ? reject.variables : undefined;

  return (
    <>
      <PageHeader
        title={t("provider.invitations.title")}
        description={t("provider.invitations.description")}
      />
      {invitations.isPending ? <LoadingState /> : null}
      {invitations.isError ? <ErrorAlert error={invitations.error} /> : null}
      {accept.isError ? <ErrorAlert error={accept.error} /> : null}
      {reject.isError ? <ErrorAlert error={reject.error} /> : null}
      {!invitations.isPending && !invitations.isError && items.length === 0 ? (
        <EmptyState
          title={t("provider.invitations.emptyTitle")}
          body={t("provider.invitations.emptyBody")}
        />
      ) : null}
      <Stack spacing={2}>
        {items.map((item) => (
          <AppCard key={item.id}>
            <Stack spacing={1.5}>
              <Typography variant="h6">{item.businessName?.trim() || `#${item.id}`}</Typography>
              <Typography variant="body1">{t("provider.invitations.body")}</Typography>
              <Stack direction="row" spacing={1} alignItems="center" sx={{ flexWrap: "wrap" }} useFlexGap>
                <Typography variant="body2" color="text.secondary">
                  {t("provider.invitations.status")}:
                </Typography>
                <StatusChip label={t("provider.invitations.pending")} tone="pending" />
              </Stack>
              <Stack direction={{ xs: "column", sm: "row" }} spacing={1}>
                <Button
                  variant="contained"
                  disabled={busyId != null}
                  onClick={() => accept.mutate(item.id)}
                >
                  {accept.isPending && accept.variables === item.id
                    ? t("provider.invitations.accepting")
                    : t("provider.invitations.accept")}
                </Button>
                <Button
                  variant="outlined"
                  color="error"
                  disabled={busyId != null}
                  onClick={() => reject.mutate(item.id)}
                >
                  {reject.isPending && reject.variables === item.id
                    ? t("provider.invitations.rejecting")
                    : t("provider.invitations.reject")}
                </Button>
              </Stack>
            </Stack>
          </AppCard>
        ))}
      </Stack>
    </>
  );
}
