import {
  Box,
  Button,
  Divider,
  MenuItem,
  Stack,
  TextField,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useEffect, useMemo, useState, type FormEvent } from "react";
import { formatDateTime, formatMoney } from "../../../../customer/proposals/model/proposalDisplay";
import { formatIsoDateForLocale, LocaleDateField, LocaleDateTimeField } from "../../../../../shared/datetime/LocaleDateField";
import { getLocale, t } from "../../../../../shared/i18n";
import { AppCard } from "../../../../../shared/ui/AppCard";
import { FormSplitLayout } from "../../../../../shared/ui/FormSplitLayout";
import type { ProviderCatalogProduct, ProviderCatalogService } from "../api/providerCatalogTypes";
import type { CreateProviderProposalBody } from "../api/createProposalTypes";
import {
  allowedItemTypes,
  createEmptyItem,
  defaultCreateProposalValues,
  itemLineTotal,
  itemsSubtotal,
  suggestedProposalTotal,
  toCreateProposalBody,
  validateCreateProposalForm,
  type CreateProposalFieldErrors,
  type CreateProposalFormValues,
  type CreateProposalItemValues,
  type ProposalItemType,
} from "../model/createProposalForm";
import { CatalogIdSelect } from "./CatalogIdSelect";

type CreateProposalFormProps = {
  requestType: string | undefined;
  services: ProviderCatalogService[];
  products: ProviderCatalogProduct[];
  servicesLoading: boolean;
  productsLoading: boolean;
  servicesFailed: boolean;
  productsFailed: boolean;
  submitting: boolean;
  disabled?: boolean;
  serverErrors?: CreateProposalFieldErrors;
  onSubmit: (body: CreateProviderProposalBody) => void;
};

function PriceRow({
  label,
  value,
  emphasize,
}: {
  label: string;
  value: string;
  emphasize?: boolean;
}) {
  return (
    <Stack direction="row" justifyContent="space-between" alignItems="baseline" spacing={2}>
      <Typography
        variant={emphasize ? "subtitle1" : "body2"}
        color={emphasize ? "text.primary" : "text.secondary"}
        fontWeight={emphasize ? 700 : 400}
      >
        {label}
      </Typography>
      <Typography
        variant={emphasize ? "h5" : "body1"}
        fontWeight={emphasize ? 800 : 600}
        sx={emphasize ? { color: "primary.main" } : undefined}
      >
        {value}
      </Typography>
    </Stack>
  );
}

export function CreateProposalForm({
  requestType,
  services,
  products,
  servicesLoading,
  productsLoading,
  servicesFailed,
  productsFailed,
  submitting,
  disabled = false,
  serverErrors,
  onSubmit,
}: CreateProposalFormProps) {
  const [values, setValues] = useState<CreateProposalFormValues>(() =>
    defaultCreateProposalValues(requestType),
  );
  const [errors, setErrors] = useState<CreateProposalFieldErrors>({});
  const theme = useTheme();
  const isMdUp = useMediaQuery(theme.breakpoints.up("md"));
  const allowed = allowedItemTypes(requestType);
  const subtotal = useMemo(() => itemsSubtotal(values), [values]);
  const computedTotal = useMemo(() => suggestedProposalTotal(values), [values]);
  const locked = submitting || disabled;
  const shownErrors = { ...serverErrors, ...errors };

  useEffect(() => {
    setValues((current) => ({
      ...current,
      totalPrice: String(computedTotal),
    }));
  }, [computedTotal]);

  useEffect(() => {
    const allowedTypes = allowedItemTypes(requestType);
    setValues((current) => ({
      ...current,
      items: current.items.map((item) =>
        allowedTypes.includes(item.itemType)
          ? item
          : {
              ...item,
              itemType: allowedTypes[0],
              serviceId: "",
              productId: "",
            },
      ),
    }));
  }, [requestType]);

  const serviceOptions = services
    .filter((item) => item.isActive)
    .map((item) => ({
      id: item.serviceId,
      label: item.serviceName?.trim() ? item.serviceName : String(item.serviceId),
    }));
  const productOptions = products
    .filter((item) => item.isAvailable)
    .map((item) => ({
      id: item.productId,
      label: item.productName?.trim() ? item.productName : String(item.productId),
    }));

  function itemName(item: CreateProposalItemValues): string {
    if (item.itemType === "Service") {
      return (
        serviceOptions.find((option) => String(option.id) === item.serviceId)?.label ??
        (item.serviceId.trim() || t("common.notSpecified"))
      );
    }
    return (
      productOptions.find((option) => String(option.id) === item.productId)?.label ??
      (item.productId.trim() || t("common.notSpecified"))
    );
  }

  function patch(update: Partial<CreateProposalFormValues>) {
    setValues((current) => ({ ...current, ...update }));
  }

  function patchItem(index: number, update: Partial<CreateProposalItemValues>) {
    setValues((current) => ({
      ...current,
      items: current.items.map((item, itemIndex) =>
        itemIndex === index ? { ...item, ...update } : item,
      ),
    }));
  }

  function handleItemType(index: number, itemType: ProposalItemType) {
    patchItem(index, { itemType, serviceId: "", productId: "" });
  }

  function handleProductSelect(index: number, productId: string) {
    const catalog = products.find((item) => String(item.productId) === productId);
    patchItem(index, {
      productId,
      unitPrice:
        catalog?.price != null && catalog.price >= 0
          ? String(catalog.price)
          : values.items[index].unitPrice,
    });
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (locked) {
      return;
    }
    const nextErrors = validateCreateProposalForm(values, requestType);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }
    onSubmit(toCreateProposalBody(values));
  }

  const submitButton = (
    <Button
      type="submit"
      variant="contained"
      size="large"
      disabled={locked}
      aria-live="polite"
      sx={{ minHeight: 48, width: "100%" }}
    >
      {submitting ? t("provider.proposalCreate.submitting") : t("provider.proposalCreate.submit")}
    </Button>
  );

  const pricingSummary = (
    <AppCard>
      <Stack spacing={2}>
        <Typography variant="h6">{t("provider.proposalCreate.pricingSummary")}</Typography>
        <Stack spacing={1.5} aria-live="polite">
          {values.items.map((item, index) => (
            <Stack key={item.key} spacing={0.25}>
              <Typography variant="body2" fontWeight={600}>
                {itemName(item)}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {t("provider.proposalCreate.qtyUnit", {
                  qty: item.quantity || "—",
                  price: formatMoney(Number(item.unitPrice) || 0),
                  total: formatMoney(itemLineTotal(item)),
                })}
              </Typography>
              {shownErrors[`items.${index}.quantity`] || shownErrors[`items.${index}.unitPrice`] ? (
                <Typography color="error" variant="caption" role="alert">
                  {shownErrors[`items.${index}.quantity`] ?? shownErrors[`items.${index}.unitPrice`]}
                </Typography>
              ) : null}
            </Stack>
          ))}
        </Stack>
        <Divider />
        <PriceRow label={t("provider.proposalCreate.subtotal")} value={formatMoney(subtotal)} />
        <TextField
          label={t("provider.proposalCreate.deliveryFee")}
          value={values.deliveryFee}
          onChange={(event) => patch({ deliveryFee: event.target.value })}
          fullWidth
          error={Boolean(shownErrors.deliveryFee)}
          helperText={shownErrors.deliveryFee}
          disabled={locked}
          inputProps={{ inputMode: "decimal" }}
        />
        <Divider />
        <PriceRow
          label={t("provider.proposalCreate.totalPrice")}
          value={formatMoney(Number(values.totalPrice) || computedTotal)}
          emphasize
        />
        <TextField
          label={t("provider.proposalCreate.totalPrice")}
          value={values.totalPrice}
          onChange={(event) => patch({ totalPrice: event.target.value })}
          required
          fullWidth
          error={Boolean(shownErrors.totalPrice)}
          helperText={shownErrors.totalPrice ?? t("provider.proposalCreate.totalHint")}
          disabled={locked}
          inputProps={{ inputMode: "decimal" }}
        />
        {values.proposedDate || values.proposedTimeFrom || values.expireAt ? (
          <Stack spacing={0.5}>
            {values.proposedDate ? (
              <Typography variant="caption" color="text.secondary">
                {t("provider.proposalCreate.proposedDate")}:{" "}
                {formatIsoDateForLocale(values.proposedDate, getLocale())}
              </Typography>
            ) : null}
            {values.expireAt ? (
              <Typography variant="caption" color="text.secondary">
                {t("provider.proposalCreate.expireAt")}:{" "}
                {formatDateTime(new Date(values.expireAt).toISOString())}
              </Typography>
            ) : null}
          </Stack>
        ) : null}
        {isMdUp ? submitButton : null}
      </Stack>
    </AppCard>
  );

  return (
    <Stack
      component="form"
      spacing={2}
      onSubmit={handleSubmit}
      noValidate
      aria-busy={submitting}
      sx={{ minWidth: 0 }}
    >
      <FormSplitLayout
        hideSummaryOnMobile
        main={
          <Stack spacing={2} sx={{ minWidth: 0 }}>
            <Typography variant="h6">{t("provider.proposalCreate.items")}</Typography>
            {shownErrors.items ? (
              <Typography color="error" variant="caption" role="alert">
                {shownErrors.items}
              </Typography>
            ) : null}

            {values.items.map((item, index) => (
              <AppCard key={item.key}>
                <Stack spacing={2}>
                  <Stack
                    direction={{ xs: "column", sm: "row" }}
                    justifyContent="space-between"
                    alignItems={{ xs: "stretch", sm: "center" }}
                    spacing={1}
                  >
                    <Typography variant="subtitle1">
                      {t("provider.proposalCreate.itemN", { n: index + 1 })}
                    </Typography>
                    {values.items.length > 1 ? (
                      <Button
                        type="button"
                        color="inherit"
                        disabled={locked}
                        onClick={() =>
                          patch({
                            items: values.items.filter((_, itemIndex) => itemIndex !== index),
                          })
                        }
                        sx={{ minHeight: 44 }}
                      >
                        {t("provider.proposalCreate.remove")}
                      </Button>
                    ) : null}
                  </Stack>

                  {allowed.length > 1 ? (
                    <TextField
                      select
                      label={t("provider.proposalCreate.itemType")}
                      value={item.itemType}
                      onChange={(event) =>
                        handleItemType(index, event.target.value as ProposalItemType)
                      }
                      disabled={locked}
                      fullWidth
                      error={Boolean(shownErrors[`items.${index}.itemType`])}
                      helperText={shownErrors[`items.${index}.itemType`]}
                    >
                      {allowed.map((type) => (
                        <MenuItem key={type} value={type}>
                          {type === "Service"
                            ? t("request.kind.Service")
                            : t("request.kind.Product")}
                        </MenuItem>
                      ))}
                    </TextField>
                  ) : (
                    <Typography variant="body2" color="text.secondary">
                      {item.itemType === "Service"
                        ? t("request.kind.Service")
                        : t("request.kind.Product")}
                    </Typography>
                  )}

                  {item.itemType === "Service" ? (
                    <CatalogIdSelect
                      label={t("provider.proposalCreate.service")}
                      value={item.serviceId}
                      options={serviceOptions}
                      loading={servicesLoading}
                      catalogFailed={servicesFailed}
                      error={shownErrors[`items.${index}.serviceId`]}
                      disabled={locked}
                      onChange={(serviceId) => patchItem(index, { serviceId })}
                    />
                  ) : (
                    <CatalogIdSelect
                      label={t("provider.proposalCreate.product")}
                      value={item.productId}
                      options={productOptions}
                      loading={productsLoading}
                      catalogFailed={productsFailed}
                      error={shownErrors[`items.${index}.productId`]}
                      disabled={locked}
                      onChange={(productId) => handleProductSelect(index, productId)}
                    />
                  )}

                  <Box
                    sx={{
                      display: "grid",
                      gap: 2,
                      gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
                      minWidth: 0,
                    }}
                  >
                    <TextField
                      label={t("provider.proposalCreate.quantity")}
                      value={item.quantity}
                      onChange={(event) => patchItem(index, { quantity: event.target.value })}
                      error={Boolean(shownErrors[`items.${index}.quantity`])}
                      helperText={shownErrors[`items.${index}.quantity`]}
                      disabled={locked}
                      required
                      fullWidth
                      inputProps={{ inputMode: "decimal" }}
                    />
                    <TextField
                      label={t("provider.proposalCreate.unitPrice")}
                      value={item.unitPrice}
                      onChange={(event) => patchItem(index, { unitPrice: event.target.value })}
                      error={Boolean(shownErrors[`items.${index}.unitPrice`])}
                      helperText={shownErrors[`items.${index}.unitPrice`]}
                      disabled={locked}
                      fullWidth
                      inputProps={{ inputMode: "decimal" }}
                    />
                  </Box>
                  <Box
                    sx={{
                      px: 1.5,
                      py: 1.25,
                      borderRadius: 1,
                      bgcolor: "action.hover",
                    }}
                  >
                    <PriceRow
                      label={t("provider.proposalCreate.itemTotal")}
                      value={formatMoney(itemLineTotal(item))}
                    />
                    <Typography variant="caption" color="text.secondary">
                      {t("provider.proposalCreate.itemTotalHint")}
                    </Typography>
                  </Box>
                  <TextField
                    label={t("provider.proposalCreate.itemDescription")}
                    value={item.description}
                    onChange={(event) =>
                      patchItem(index, { description: event.target.value.slice(0, 2000) })
                    }
                    error={Boolean(shownErrors[`items.${index}.description`])}
                    helperText={
                      shownErrors[`items.${index}.description`] ?? `${item.description.length}/2000`
                    }
                    disabled={locked}
                    fullWidth
                    multiline
                    minRows={2}
                  />
                </Stack>
              </AppCard>
            ))}

            <Button
              type="button"
              variant="outlined"
              disabled={locked}
              onClick={() => patch({ items: [...values.items, createEmptyItem(requestType)] })}
              sx={{ minHeight: 48, width: { xs: "100%", sm: "auto" }, alignSelf: { sm: "flex-start" } }}
            >
              {t("provider.proposalCreate.addItem")}
            </Button>

            <AppCard>
              <Stack spacing={2}>
                <Typography variant="subtitle1">{t("provider.proposalCreate.schedule")}</Typography>
                <Box
                  sx={{
                    display: "grid",
                    gap: 2,
                    gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
                    minWidth: 0,
                  }}
                >
                  <LocaleDateField
                    label={t("provider.proposalCreate.proposedDate")}
                    value={values.proposedDate}
                    onChange={(isoDate) => patch({ proposedDate: isoDate })}
                    disabled={locked}
                  />
                  <LocaleDateTimeField
                    label={t("provider.proposalCreate.expireAt")}
                    value={values.expireAt}
                    onChange={(isoDateTime) => patch({ expireAt: isoDateTime })}
                    error={Boolean(shownErrors.expireAt)}
                    helperText={shownErrors.expireAt}
                    disabled={locked}
                  />
                  <TextField
                    label={t("provider.proposalCreate.timeFrom")}
                    type="time"
                    value={values.proposedTimeFrom}
                    onChange={(event) => patch({ proposedTimeFrom: event.target.value })}
                    error={Boolean(shownErrors.proposedTimeFrom)}
                    helperText={shownErrors.proposedTimeFrom}
                    disabled={locked}
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                  />
                  <TextField
                    label={t("provider.proposalCreate.timeTo")}
                    type="time"
                    value={values.proposedTimeTo}
                    onChange={(event) => patch({ proposedTimeTo: event.target.value })}
                    error={Boolean(shownErrors.proposedTimeTo)}
                    helperText={shownErrors.proposedTimeTo}
                    disabled={locked}
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                  />
                </Box>
              </Stack>
            </AppCard>

            <AppCard>
              <Stack spacing={2}>
                <Typography variant="subtitle1">{t("provider.proposalCreate.message")}</Typography>
                <TextField
                  label={t("provider.proposalCreate.message")}
                  value={values.message}
                  onChange={(event) => patch({ message: event.target.value.slice(0, 2000) })}
                  fullWidth
                  multiline
                  minRows={3}
                  error={Boolean(shownErrors.message)}
                  helperText={shownErrors.message ?? `${values.message.length}/2000`}
                  disabled={locked}
                />
              </Stack>
            </AppCard>
          </Stack>
        }
        summary={pricingSummary}
      />
      {!isMdUp ? (
      <Box
        sx={{
          position: "sticky",
          bottom: 0,
          bgcolor: "background.paper",
          py: 1.5,
          zIndex: 2,
          borderTop: 1,
          borderColor: "divider",
        }}
      >
        {submitButton}
      </Box>
      ) : null}
    </Stack>
  );
}
