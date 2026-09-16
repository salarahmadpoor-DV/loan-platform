import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel, type RequestDto } from "../api/requestTypes";
import {
  formatRequestDate,
  isRequestOpen,
  requestDescriptionSnippet,
  requestLocationSummary,
} from "../model/requestPresentation";
import { RequestStatusChip } from "./RequestStatusChip";

type RequestCardProps = {
  request: RequestDto;
};

export function RequestCard({ request }: RequestCardProps) {
  const place = requestLocationSummary(request.location);
  const snippet = requestDescriptionSnippet(request.description);
  const open = isRequestOpen(request.status);

  return (
    <AppCard
      sx={{
        height: "100%",
        "&:hover": { borderColor: "primary.light" },
      }}
    >
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          spacing={1}
          alignItems="flex-start"
          justifyContent="space-between"
          sx={{ flexWrap: "wrap" }}
          useFlexGap
        >
          <Typography variant="subtitle1" fontWeight={700} sx={{ minWidth: 0, flex: 1 }}>
            {request.title}
          </Typography>
          <RequestStatusChip status={request.status} />
        </Stack>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <StatusChip label={requestKindLabel(request.requestType)} tone="info" />
          <Typography variant="caption" color="text.secondary" sx={{ alignSelf: "center" }}>
            {t("request.card.id", { id: request.id })}
          </Typography>
        </Stack>
        {snippet ? (
          <Typography variant="body2" color="text.secondary">
            {snippet}
          </Typography>
        ) : null}
        {place ? (
          <Typography variant="body2" color="text.secondary">
            {place}
          </Typography>
        ) : null}
        <Typography variant="caption" color="text.secondary">
          {t("request.card.created", { date: formatRequestDate(request.createDate) })}
          {" · "}
          {t("request.card.counts", {
            services: request.services.length,
            products: request.products.length,
          })}
        </Typography>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          spacing={1}
          sx={{ mt: "auto", pt: 0.5 }}
        >
          <Button
            component={RouterLink}
            to={`/customer/requests/${request.id}`}
            variant="contained"
            sx={{ minHeight: 44, width: { xs: "100%", sm: "auto" } }}
          >
            {t("request.card.view")}
          </Button>
          {open ? (
            <Button
              component={RouterLink}
              to={`/customer/requests/${request.id}/matches`}
              variant="outlined"
              sx={{ minHeight: 44, width: { xs: "100%", sm: "auto" } }}
            >
              {t("request.card.viewMatches")}
            </Button>
          ) : null}
        </Stack>
      </Stack>
    </AppCard>
  );
}
