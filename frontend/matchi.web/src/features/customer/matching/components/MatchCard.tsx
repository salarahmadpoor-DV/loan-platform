import { Avatar, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { isMatchCandidateType, type MatchResult } from "../api/matchingTypes";
import { MatchScoreChip } from "./MatchScoreChip";

type MatchCardProps = {
  match: MatchResult;
};

function formatDistanceKm(km: number): string {
  return km.toLocaleString(undefined, { maximumFractionDigits: 1, minimumFractionDigits: 0 });
}

function initials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) {
    return "?";
  }
  if (parts.length === 1) {
    return parts[0].slice(0, 2);
  }
  return `${parts[0].slice(0, 1)}${parts[parts.length - 1].slice(0, 1)}`;
}

export function MatchCard({ match }: MatchCardProps) {
  const typeLabel = isMatchCandidateType(match.candidateType)
    ? t(match.candidateType === "Business" ? "matching.type.Business" : "matching.type.Provider")
    : match.candidateType;

  return (
    <AppCard>
      <Stack spacing={1.5} direction="row" alignItems="flex-start">
        <Avatar
          alt=""
          sx={{
            bgcolor: match.candidateType === "Business" ? "secondary.main" : "primary.main",
            color: match.candidateType === "Business" ? "secondary.contrastText" : "primary.contrastText",
            fontWeight: 700,
          }}
        >
          {initials(match.displayName)}
        </Avatar>
        <Stack spacing={1} sx={{ minWidth: 0, flex: 1 }}>
          <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
            <StatusChip label={typeLabel} tone={match.candidateType === "Business" ? "info" : "primary"} />
            <StatusChip label={t("matching.rank", { rank: match.rank })} />
            <MatchScoreChip score={match.score} />
          </Stack>
          <Typography variant="subtitle1" fontWeight={700}>
            {match.displayName}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {t("matching.candidateId", { id: match.candidateId })}
          </Typography>
          {match.distanceKm != null ? (
            <Typography variant="body2" color="text.secondary">
              {t("matching.distanceKm", { km: formatDistanceKm(match.distanceKm) })}
            </Typography>
          ) : null}
        </Stack>
      </Stack>
    </AppCard>
  );
}
