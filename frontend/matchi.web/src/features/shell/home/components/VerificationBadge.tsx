import { Chip } from "@mui/material";
import { t } from "../../../../shared/i18n";

export function VerificationBadge() {
  return (
    <Chip
      size="small"
      color="secondary"
      label={t("public.pros.previewBadge")}
      sx={{ alignSelf: "flex-start" }}
    />
  );
}
