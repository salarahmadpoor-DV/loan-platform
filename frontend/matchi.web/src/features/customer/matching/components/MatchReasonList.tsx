import { List, ListItem, ListItemText, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import {
  MATCH_SCORE_SIGNALS,
  isMatchCandidateType,
} from "../api/matchingTypes";

type MatchReasonListProps = {
  candidateType: string;
};

export function MatchReasonList({ candidateType }: MatchReasonListProps) {
  const isProvider = isMatchCandidateType(candidateType) && candidateType === "Provider";
  const signals = MATCH_SCORE_SIGNALS.filter((signal) => !signal.providerOnly || isProvider);

  return (
    <>
      <Typography variant="subtitle2">{t("matching.reasonsTitle")}</Typography>
      <Typography variant="caption" color="text.secondary" display="block" sx={{ mb: 0.5 }}>
        {t("matching.reasonsHint")}
      </Typography>
      <List dense disablePadding>
        {signals.map((signal) => (
          <ListItem key={signal.key} disableGutters sx={{ py: 0 }}>
            <ListItemText primary={t(signal.labelKey)} primaryTypographyProps={{ variant: "body2" }} />
          </ListItem>
        ))}
      </List>
    </>
  );
}
