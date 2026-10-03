import { Box, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { OverflowActions } from "../../../../shared/ui/OverflowActions";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { BusinessMembership } from "../api/ownedBusinessApi";

function membershipTone(status: string) {
  const value = status.toLowerCase();
  if (value === "active") {
    return "success" as const;
  }
  if (value === "pending") {
    return "pending" as const;
  }
  if (value === "inactive") {
    return "neutral" as const;
  }
  return "info" as const;
}

type MembershipCardProps = {
  membership: BusinessMembership;
  onRemove?: (providerId: number) => void;
  removing?: boolean;
};

export function MembershipCard({ membership, onRemove, removing }: MembershipCardProps) {
  const joined = membership.joinedAt
    ? new Date(membership.joinedAt).toLocaleDateString()
    : null;

  return (
    <AppCard>
      <Stack spacing={1}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap alignItems="center">
          <Typography variant="subtitle1">
            {membership.providerName?.trim() || `#${membership.providerId}`}
          </Typography>
          <StatusChip label={membership.status} tone={membershipTone(membership.status)} />
        </Stack>
        <Typography variant="body2" color="text.secondary">
          {t("provider.business.role")}: {membership.role}
        </Typography>
        {joined ? (
          <Typography variant="body2" color="text.secondary">
            {t("provider.business.joinedAt", { date: joined })}
          </Typography>
        ) : null}
        {onRemove ? (
          <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
            <OverflowActions
              items={[
                {
                  key: "remove",
                  label: t("provider.business.remove"),
                  destructive: true,
                  disabled: removing,
                  onClick: () => onRemove(membership.providerId),
                },
              ]}
            />
          </Box>
        ) : null}
      </Stack>
    </AppCard>
  );
}
