import { Box, Button, Stack, Typography } from "@mui/material";
import type { ReactNode } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../../../../shared/api/errors";
import { t } from "../../../../shared/i18n";
import { buildCustomerJourney } from "../../../../shared/marketplace/customerJourney";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { JourneyTimeline } from "../../../../shared/ui/JourneyTimeline";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import {
  requestKindLabel,
  type RequestDto,
  type RequestProductLine,
  type RequestServiceLine,
} from "../api/requestTypes";
import { RequestStatusChip } from "../components/RequestStatusChip";
import { useRequest } from "../hooks/useRequest";
import { useRequestProposals } from "../../proposals/hooks/useRequestProposals";
import {
  formatRequestDateTime,
  isRequestOpen,
} from "../model/requestPresentation";

function parseRequestId(raw: string | undefined): number | undefined {
  if (!raw) {
    return undefined;
  }
  const id = Number.parseInt(raw, 10);
  return Number.isFinite(id) && id > 0 ? id : undefined;
}

function locationLines(request: RequestDto): string[] {
  const location = request.location;
  if (!location) {
    return [];
  }
  return [
    location.address,
    [location.district, location.city, location.province].filter(Boolean).join(", "),
  ].filter((line): line is string => Boolean(line && line.trim()));
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
        action={
          <Button
            component={RouterLink}
            to="/customer/requests"
            variant="outlined"
            sx={{ minHeight: 48 }}
          >
            {t("request.detail.backToList")}
          </Button>
        }
      />
      {isPending ? <LoadingState label={t("request.detail.loading")} /> : null}
      {isError ? <ErrorAlert error={error} /> : null}
      {data ? <RequestDetailBody request={data} /> : null}
    </>
  );
}

function RequestDetailBody({ request }: { request: RequestDto }) {
  const places = locationLines(request);
  const open = isRequestOpen(request.status);
  const proposalsQuery = useRequestProposals(request.id);

  return (
    <Stack spacing={2}>
      <JourneyTimeline
        steps={buildCustomerJourney({
          current: "request",
          requestId: request.id,
          requestExists: true,
          proposalsLoaded: Boolean(proposalsQuery.data) && !proposalsQuery.isError,
          hasProposals: (proposalsQuery.data?.length ?? 0) > 0,
          proposalAccepted:
            proposalsQuery.data?.some((item) => item.status.toLowerCase() === "accepted") ?? false,
        })}
      />
      <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
        <StatusChip label={requestKindLabel(request.requestType)} tone="info" />
        <RequestStatusChip status={request.status} />
        <Typography variant="caption" color="text.secondary" sx={{ alignSelf: "center" }}>
          {t("request.card.id", { id: request.id })}
        </Typography>
      </Stack>

      <Stack direction={{ xs: "column", sm: "row" }} spacing={1}>
        {open ? (
          <Button
            component={RouterLink}
            to={`/customer/requests/${request.id}/matches`}
            variant="contained"
            sx={{ minHeight: 48 }}
          >
            {t("request.detail.viewMatches")}
          </Button>
        ) : null}
        <Button
          component={RouterLink}
          to={`/customer/requests/${request.id}/proposals`}
          variant={open ? "outlined" : "contained"}
          sx={{ minHeight: 48 }}
        >
          {t("request.detail.viewProposals")}
        </Button>
      </Stack>

      <AppCard>
        <Typography variant="subtitle2" color="text.secondary">
          {t("request.detail.about")}
        </Typography>
        <Typography variant="body2" sx={{ mt: 1, whiteSpace: "pre-wrap" }}>
          {request.description?.trim() ? request.description : t("request.detail.noDescription")}
        </Typography>
        <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 2 }}>
          {t("request.detail.created", { date: formatRequestDateTime(request.createDate) })}
        </Typography>
        {request.updateDate ? (
          <Typography variant="caption" color="text.secondary" display="block">
            {t("request.detail.updated", { date: formatRequestDateTime(request.updateDate) })}
          </Typography>
        ) : null}
      </AppCard>

      <Box
        sx={{
          display: "grid",
          gap: 2,
          gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" },
        }}
      >
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
      </Box>

      <LineSection
        title={t("request.detail.services")}
        emptyTitle={t("request.detail.noServicesTitle")}
        emptyBody={
          request.requestType === "Product"
            ? t("request.detail.noServicesProduct")
            : t("request.detail.noServicesGeneric")
        }
        items={request.services.map((line) => (
          <ServiceLineCard key={line.id} line={line} />
        ))}
      />

      <LineSection
        title={t("request.detail.products")}
        emptyTitle={t("request.detail.noProductsTitle")}
        emptyBody={
          request.requestType === "Service"
            ? t("request.detail.noProductsService")
            : t("request.detail.noProductsGeneric")
        }
        items={request.products.map((line) => (
          <ProductLineCard key={line.id} line={line} />
        ))}
      />
    </Stack>
  );
}

function LineSection({
  title,
  emptyTitle,
  emptyBody,
  items,
}: {
  title: string;
  emptyTitle: string;
  emptyBody: string;
  items: ReactNode[];
}) {
  return (
    <Box>
      <Typography variant="subtitle1" sx={{ mb: 1 }}>
        {title}
      </Typography>
      {items.length === 0 ? (
        <AppCard>
          <Typography variant="body2">{emptyTitle}</Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            {emptyBody}
          </Typography>
        </AppCard>
      ) : (
        <Stack spacing={1}>{items}</Stack>
      )}
    </Box>
  );
}

function AttributeList({
  items,
}: {
  items: Array<{ id: number; value: string | null }>;
}) {
  if (items.length === 0) {
    return null;
  }
  return (
    <Stack spacing={0.25} sx={{ mt: 1 }}>
      {items.map((item) => (
        <Typography key={item.id} variant="caption" color="text.secondary">
          {t("request.detail.attribute", { id: item.id })}
          {item.value ? `: ${item.value}` : ""}
        </Typography>
      ))}
    </Stack>
  );
}

function ServiceLineCard({ line }: { line: RequestServiceLine }) {
  return (
    <AppCard>
      <Typography variant="body2" fontWeight={600}>
        {t("request.detail.serviceId", { id: line.serviceId })}
        {" · "}
        {t("request.detail.quantity", { value: line.quantity })}
      </Typography>
      {line.description ? (
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {line.description}
        </Typography>
      ) : null}
      <AttributeList
        items={line.attributes.map((attribute) => ({
          id: attribute.serviceAttributeId,
          value: attribute.value,
        }))}
      />
    </AppCard>
  );
}

function ProductLineCard({ line }: { line: RequestProductLine }) {
  const ref =
    line.productId != null
      ? t("request.detail.productId", { id: line.productId })
      : line.productCategoryId != null
        ? t("request.detail.categoryId", { id: line.productCategoryId })
        : t("common.notSpecified");

  return (
    <AppCard>
      <Typography variant="body2" fontWeight={600}>
        {ref}
        {" · "}
        {t("request.detail.quantity", { value: line.quantity })}
        {line.unit ? ` ${line.unit}` : ""}
      </Typography>
      {line.description ? (
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {line.description}
        </Typography>
      ) : null}
      <AttributeList
        items={line.attributes.map((attribute) => ({
          id: attribute.productAttributeId,
          value: attribute.value,
        }))}
      />
    </AppCard>
  );
}
