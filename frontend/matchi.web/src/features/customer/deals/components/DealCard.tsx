import { Button, Stack } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import type { DealSummary } from "../api/dealTypes";
import { DealSummary as DealSummaryBlock } from "./DealSummary";

type DealCardProps = {
  deal: DealSummary;
};

export function DealCard({ deal }: DealCardProps) {
  return (
    <AppCard>
      <Stack spacing={1.5}>
        <DealSummaryBlock deal={deal} />
        <Button
          component={RouterLink}
          to={`/customer/deals/${deal.id}`}
          variant="outlined"
          sx={{ alignSelf: "flex-start" }}
        >
          {t("deal.view")}
        </Button>
      </Stack>
    </AppCard>
  );
}
