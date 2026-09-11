import { Chip } from "@mui/material";
import { t } from "../../../../shared/i18n";

type MatchScoreChipProps = {
  score: number;
};

export function MatchScoreChip({ score }: MatchScoreChipProps) {
  const color = score >= 50 ? "success" : score >= 20 ? "info" : "default";

  return (
    <Chip
      size="small"
      color={color}
      label={`${t("matching.score")}: ${score}`}
    />
  );
}
