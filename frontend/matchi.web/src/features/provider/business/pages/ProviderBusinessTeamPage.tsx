import { Button, Stack } from "@mui/material";
import { useState } from "react";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";
import { AddProviderDialog } from "../components/AddProviderDialog";
import { MembershipCard } from "../components/MembershipCard";
import { OwnedBusinessPage } from "../components/OwnedBusinessPage";
import { useRemoveBusinessProvider } from "../hooks/useBusinessTeamMutations";
import { useOwnedBusinessTeam } from "../hooks/useOwnedBusinessTeam";

function TeamBody({ business }: { business: MyBusinessProfile }) {
  const team = useOwnedBusinessTeam(business.id);
  const remove = useRemoveBusinessProvider(business.id);
  const [inviteOpen, setInviteOpen] = useState(false);
  const members = (team.data ?? []).filter((item) => item.status.toLowerCase() === "active");

  function onRemove(providerId: number) {
    if (!window.confirm(t("provider.business.removeConfirm"))) {
      return;
    }
    remove.mutate(providerId);
  }

  return (
    <>
      <Stack spacing={2}>
        <Button
          variant="contained"
          onClick={() => setInviteOpen(true)}
          sx={{ alignSelf: "flex-start", minHeight: 44 }}
        >
          {t("provider.business.addProvider")}
        </Button>
        {remove.isError ? <ErrorAlert error={remove.error} /> : null}
        {team.isPending ? <LoadingState /> : null}
        {team.isError ? <ErrorAlert error={team.error} /> : null}
        {!team.isPending && members.length === 0 ? (
          <EmptyState title={t("provider.business.emptyTeam")} />
        ) : null}
        {members.map((membership) => (
          <MembershipCard
            key={membership.providerId}
            membership={membership}
            onRemove={onRemove}
            removing={remove.isPending && remove.variables === membership.providerId}
          />
        ))}
      </Stack>
      <AddProviderDialog open={inviteOpen} businessId={business.id} onClose={() => setInviteOpen(false)} />
    </>
  );
}

export function ProviderBusinessTeamPage() {
  return (
    <OwnedBusinessPage
      title={t("provider.business.teamTitle")}
      description={t("provider.business.teamDescription")}
    >
      {(business) => <TeamBody business={business} />}
    </OwnedBusinessPage>
  );
}
