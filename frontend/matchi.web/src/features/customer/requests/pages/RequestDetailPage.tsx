import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t, getLocale } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { requestKindLabel, type RequestDto } from "../api/requestTypes";
import { RequestStatusChip } from "../components/RequestStatusChip";
import { useRequest } from "../hooks/useRequest";

function parseRequestId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}

function formatWhen(iso: string | null | undefined): string {
  if (!iso) {
    return "—";
  }
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return iso;
  }
  return date.toLocaleString(getLocale());
}

function locationLines(request: RequestDto): string[] {
  const location = request.location;
  if (!location) {
    return [];
  }
  return [
    location.address,
    [location.district, location.city, location.province].filter(Boolean).join(", "),
  ].filter((line): line is string => Boolean(line));
}

export function RequestDetailPage() {
  const { id } = useParams();
  const requestId = parseRequestId(id);
  const { data, isPending, isError, error } = useRequest(requestId);

  if (requestId == null) {
    return (
      <ErrorAlert
        error={new ApiError({ status: 400, userMessage: t("request.detail.invalidLink") })}
      />
    );
  }

  return (
    <>
      <PageHeader
        title={data?.title ?? t("request.detail.fallbackTitle")}
        description={t("request.detail.description")}
      />
      {isPending ? <LoadingState label={t("request.detail.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}
      {data ? <RequestDetailBody request={data} /> : null}
    </>
  );
}

function RequestDetailBody({ request }: { request: RequestDto }) {
  const places = locationLines(request);

  return (
    <Stack spacing={2}>
      <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
        <StatusChip label={requestKindLabel(request.requestType)} />
        <RequestStatusChip status={request.status} />
      </Stack>

      {request.status.toLowerCase() !== "cancelled" ? (
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          <Button
            component={RouterLink}
            to={`/customer/requests/${request.id}/matches`}
            variant="contained"
          >
            {t("request.detail.viewMatches")}
          </Button>
          <Button
            component={RouterLink}
            to={`/customer/requests/${request.id}/proposals`}
            variant="outlined"
          >
            {t("request.detail.viewProposals")}
          </Button>
        </Stack>
      ) : null}

      <AppCard>
        <Typography variant="subtitle2" color="text.secondary">
          {t("request.detail.about")}
        </Typography>
        <Typography variant="body2" sx={{ mt: 1, whiteSpace: "pre-wrap" }}>
          {request.description?.trim() ? request.description : t("request.detail.noDescription")}
        </Typography>
        <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 2 }}>
          {t("request.detail.created", { date: formatWhen(request.createDate) })}
        </Typography>
      </AppCard>

      <AppCard>
        <Typography variant="subtitle2" gutterBottom>
          {t("request.detail.location")}
        </Typography>
        {places.length === 0 ? (
          <Typography variant="body2" color="text.secondary">
            {t("request.detail.noLocation")}
          </Typography>
        ) : (
          places.map((line) => (
            <Typography key={line} variant="body2">
              {line}
            </Typography>
          ))
        )}
      </AppCard>

      <AppCard>
        <Typography variant="subtitle2" gutterBottom>
          {t("request.detail.schedule")}
        </Typography>
        {request.schedule ? (
          <Typography variant="body2">
            {request.schedule.date}
            {request.schedule.timeFrom || request.schedule.timeTo
              ? ` · ${request.schedule.timeFrom ?? "?"} – ${request.schedule.timeTo ?? "?"}`
              : ""}
            {request.schedule.isFlexible ? ` · ${t("request.detail.flexible")}` : ""}
          </Typography>
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("request.detail.noSchedule")}
          </Typography>
        )}
      </AppCard>

      <Box>
        <Typography variant="subtitle1" sx={{ mb: 1 }}>
          {t("request.detail.services")}
        </Typography>
        {request.services.length === 0 ? (
          <EmptyState
            title={t("request.detail.noServicesTitle")}
            body={
              request.requestType === "Product"
                ? t("request.detail.noServicesProduct")
                : t("request.detail.noServicesGeneric")
            }
          />
        ) : (
          <Stack spacing={1}>
            {request.services.map((line) => (
              <AppCard key={line.id}>
                <Typography variant="body2">
                  Service #{line.serviceId} · qty {line.quantity}
                </Typography>
                {line.description ? (
                  <Typography variant="body2" color="text.secondary">
                    {line.description}
                  </Typography>
                ) : null}
              </AppCard>
            ))}
          </Stack>
        )}
      </Box>

      <Box>
        <Typography variant="subtitle1" sx={{ mb: 1 }}>
          {t("request.detail.products")}
        </Typography>
        {request.products.length === 0 ? (
          <EmptyState
            title={t("request.detail.noProductsTitle")}
            body={
              request.requestType === "Service"
                ? t("request.detail.noProductsService")
                : t("request.detail.noProductsGeneric")
            }
          />
        ) : (
          <Stack spacing={1}>
            {request.products.map((line) => (
              <AppCard key={line.id}>
                <Typography variant="body2">
                  {line.productId != null
                    ? `Product #${line.productId}`
                    : `Category #${line.productCategoryId ?? "—"}`}
                  {" · "}
                  qty {line.quantity}
                  {line.unit ? ` ${line.unit}` : ""}
                </Typography>
                {line.description ? (
                  <Typography variant="body2" color="text.secondary">
                    {line.description}
                  </Typography>
                ) : null}
              </AppCard>
            ))}
          </Stack>
        )}
      </Box>
    </Stack>
  );
}
