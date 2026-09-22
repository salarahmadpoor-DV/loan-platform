import { Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";
import { MembershipCard } from "../components/MembershipCard";
import { OwnedBusinessPage } from "../components/OwnedBusinessPage";
import { useRemoveBusinessProvider } from "../hooks/useBusinessTeamMutations";
import { useOwnedBusinessTeam } from "../hooks/useOwnedBusinessTeam";

function InvitationsBody({ business }: { business: MyBusinessProfile }) {
  const team = useOwnedBusinessTeam(business.id);
  const remove = useRemoveBusinessProvider(business.id);
  const pending = (team.data ?? []).filter((item) => item.status.toLowerCase() === "pending");

  function onRemove(providerId: number) {
    if (!window.confirm(t("provider.business.removeConfirm"))) {
      return;
    }
    remove.mutate(providerId);
  }

  return (
    <Stack spacing={2}>
      {remove.isError ? <ErrorAlert error={remove.error} /> : null}
      {team.isPending ? <LoadingState /> : null}
      {team.isError ? <ErrorAlert error={team.error} /> : null}
      {!team.isPending && pending.length === 0 ? (
        <EmptyState title={t("provider.business.emptyInvites")} />
      ) : null}
      {pending.map((membership) => (
        <MembershipCard
          key={membership.providerId}
          membership={membership}
          onRemove={onRemove}
          removing={remove.isPending && remove.variables === membership.providerId}
        />
      ))}
    </Stack>
  );
}

export function ProviderBusinessInvitationsPage() {
  return (
    <OwnedBusinessPage
      title={t("provider.business.invitationsTitle")}
      description={t("provider.business.invitationsDescription")}
    >
      {(business) => <InvitationsBody business={business} />}
    </OwnedBusinessPage>
  );
}
