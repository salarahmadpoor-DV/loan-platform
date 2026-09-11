import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { type MatchResult } from "../api/matchingTypes";
import { MatchReasonList } from "./MatchReasonList";
import { MatchScoreChip } from "./MatchScoreChip";

type MatchCardProps = {
  match: MatchResult;
};

export function MatchCard({ match }: MatchCardProps) {
  const typeLabel =
    match.candidateType === "Business"
      ? t("matching.type.Business")
      : match.candidateType === "Provider"
        ? t("matching.type.Provider")
        : match.candidateType;
  const nextLabel =
    match.candidateType === "Business"
      ? t("matching.viewBusiness")
      : t("matching.viewProvider");

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <StatusChip label={typeLabel} />
          <MatchScoreChip score={match.score} />
          <StatusChip label={t("matching.rank", { rank: match.rank })} />
        </Stack>
        <Typography variant="subtitle1">{match.displayName}</Typography>
        <MatchReasonList candidateType={match.candidateType} />
        <Button variant="outlined" disabled sx={{ alignSelf: "flex-start" }}>
          {nextLabel}
        </Button>
        <Typography variant="caption" color="text.secondary">
          {t("matching.nextActionHint")}
        </Typography>
      </Stack>
    </AppCard>
  );
}
