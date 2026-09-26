import { Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { useMyProviderMemberships } from "../../profile/hooks/useMyProviderMemberships";

function isActiveMembership(status: string): boolean {
  return status.toLowerCase() === "active";
}

export function ProviderWorkingBusinessesPage() {
  const memberships = useMyProviderMemberships();
  const active = (memberships.data ?? []).filter((item) => isActiveMembership(item.status));

  return (
    <>
      <PageHeader
        title={t("provider.memberships.title")}
        description={t("provider.memberships.description")}
      />
      {memberships.isPending ? <LoadingState /> : null}
      {memberships.isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={memberships.error} />
        </Stack>
      ) : null}
      {!memberships.isPending && !memberships.isError && active.length === 0 ? (
        <EmptyState
          title={t("provider.memberships.emptyTitle")}
          body={t("provider.memberships.emptyBody")}
        />
      ) : null}
      <Stack spacing={2}>
        {active.map((item) => {
          const joined = item.joinedAt ? new Date(item.joinedAt).toLocaleDateString() : null;
          return (
            <AppCard key={`${item.businessId}-${item.joinedAt}`}>
              <Stack spacing={1}>
                <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap alignItems="center">
                  <Typography variant="h6">
                    {item.businessName?.trim() || t("provider.profile.membershipBusiness", { id: item.businessId })}
                  </Typography>
                  <StatusChip label={item.status} tone="success" />
                </Stack>
                <Typography variant="body2" color="text.secondary">
                  {t("provider.memberships.role", { role: item.role })}
                </Typography>
                {joined ? (
                  <Typography variant="body2" color="text.secondary">
                    {t("provider.memberships.joinedAt", { date: joined })}
                  </Typography>
                ) : null}
              </Stack>
            </AppCard>
          );
        })}
      </Stack>
    </>
  );
}
