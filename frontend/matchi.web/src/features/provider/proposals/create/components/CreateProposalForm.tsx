import {
  Button,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useMemo, useState, type FormEvent } from "react";
import { t } from "../../../../../shared/i18n";
import { AppCard } from "../../../../../shared/ui/AppCard";
import type { ProviderCatalogProduct, ProviderCatalogService } from "../api/providerCatalogTypes";
import type { CreateProviderProposalBody } from "../api/createProposalTypes";
import {
  allowedItemTypes,
  createEmptyItem,
  defaultCreateProposalValues,
  itemLineTotal,
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
  onSubmit: (body: CreateProviderProposalBody) => void;
};

export function CreateProposalForm({
  requestType,
  services,
  products,
  servicesLoading,
  productsLoading,
  servicesFailed,
  productsFailed,
  submitting,
  onSubmit,
}: CreateProposalFormProps) {
  const [values, setValues] = useState<CreateProposalFormValues>(() =>
    defaultCreateProposalValues(requestType),
  );
  const [errors, setErrors] = useState<CreateProposalFieldErrors>({});
  const allowed = allowedItemTypes(requestType);
  const computedTotal = useMemo(() => suggestedProposalTotal(values), [values]);

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
        catalog?.price != null && catalog.price >= 0 ? String(catalog.price) : values.items[index].unitPrice,
    });
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (submitting) {
      return;
    }
    const nextErrors = validateCreateProposalForm(values, requestType);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }
    onSubmit(toCreateProposalBody(values));
  }

  return (
    <Stack component="form" spacing={2} onSubmit={handleSubmit} noValidate>
      <AppCard>
        <Stack spacing={2}>
          <Typography variant="subtitle1">{t("provider.proposalCreate.pricing")}</Typography>
          <TextField
            label={t("provider.proposalCreate.totalPrice")}
            value={values.totalPrice}
            onChange={(event) => patch({ totalPrice: event.target.value })}
            required
            fullWidth
            error={Boolean(errors.totalPrice)}
            helperText={errors.totalPrice ?? t("provider.proposalCreate.totalHint")}
            disabled={submitting}
            inputProps={{ inputMode: "decimal" }}
          />
          <TextField
            label={t("provider.proposalCreate.deliveryFee")}
            value={values.deliveryFee}
            onChange={(event) => patch({ deliveryFee: event.target.value })}
            fullWidth
            error={Boolean(errors.deliveryFee)}
            helperText={errors.deliveryFee}
            disabled={submitting}
            inputProps={{ inputMode: "decimal" }}
          />
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
            error={Boolean(errors.message)}
            helperText={errors.message ?? `${values.message.length}/2000`}
            disabled={submitting}
          />
        </Stack>
      </AppCard>

      <AppCard>
        <Stack spacing={2}>
          <Typography variant="subtitle1">{t("provider.proposalCreate.schedule")}</Typography>
          <TextField
            label={t("provider.proposalCreate.proposedDate")}
            type="date"
            value={values.proposedDate}
            onChange={(event) => patch({ proposedDate: event.target.value })}
            disabled={submitting}
            fullWidth
            InputLabelProps={{ shrink: true }}
          />
          <TextField
            label={t("provider.proposalCreate.timeFrom")}
            type="time"
            value={values.proposedTimeFrom}
            onChange={(event) => patch({ proposedTimeFrom: event.target.value })}
            error={Boolean(errors.proposedTimeFrom)}
            helperText={errors.proposedTimeFrom}
            disabled={submitting}
            fullWidth
            InputLabelProps={{ shrink: true }}
          />
          <TextField
            label={t("provider.proposalCreate.timeTo")}
            type="time"
            value={values.proposedTimeTo}
            onChange={(event) => patch({ proposedTimeTo: event.target.value })}
            error={Boolean(errors.proposedTimeTo)}
            helperText={errors.proposedTimeTo}
            disabled={submitting}
            fullWidth
            InputLabelProps={{ shrink: true }}
          />
        </Stack>
      </AppCard>

      <AppCard>
        <Stack spacing={2}>
          <Typography variant="subtitle1">{t("provider.proposalCreate.expireAt")}</Typography>
          <TextField
            label={t("provider.proposalCreate.expireAt")}
            type="datetime-local"
            value={values.expireAt}
            onChange={(event) => patch({ expireAt: event.target.value })}
            error={Boolean(errors.expireAt)}
            helperText={errors.expireAt}
            disabled={submitting}
            fullWidth
            InputLabelProps={{ shrink: true }}
          />
        </Stack>
      </AppCard>

      <Typography variant="subtitle1">{t("provider.proposalCreate.items")}</Typography>
      {errors.items ? (
        <Typography color="error" variant="caption">
          {errors.items}
        </Typography>
      ) : null}

      {values.items.map((item, index) => (
        <AppCard key={item.key}>
          <Stack spacing={2}>
            <Stack direction="row" justifyContent="space-between" alignItems="center">
              <Typography variant="subtitle2">
                {t("provider.proposalCreate.itemN", { n: index + 1 })}
              </Typography>
              {values.items.length > 1 ? (
                <Button
                  type="button"
                  color="inherit"
                  disabled={submitting}
                  onClick={() =>
                    patch({ items: values.items.filter((_, itemIndex) => itemIndex !== index) })
                  }
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
                disabled={submitting}
                fullWidth
                error={Boolean(errors[`items.${index}.itemType`])}
                helperText={errors[`items.${index}.itemType`]}
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
                error={errors[`items.${index}.serviceId`]}
                disabled={submitting}
                onChange={(serviceId) => patchItem(index, { serviceId })}
              />
            ) : (
              <CatalogIdSelect
                label={t("provider.proposalCreate.product")}
                value={item.productId}
                options={productOptions}
                loading={productsLoading}
                catalogFailed={productsFailed}
                error={errors[`items.${index}.productId`]}
                disabled={submitting}
                onChange={(productId) => handleProductSelect(index, productId)}
              />
            )}

            <TextField
              label={t("provider.proposalCreate.quantity")}
              value={item.quantity}
              onChange={(event) => patchItem(index, { quantity: event.target.value })}
              error={Boolean(errors[`items.${index}.quantity`])}
              helperText={errors[`items.${index}.quantity`]}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "decimal" }}
            />
            <TextField
              label={t("provider.proposalCreate.unitPrice")}
              value={item.unitPrice}
              onChange={(event) => patchItem(index, { unitPrice: event.target.value })}
              error={Boolean(errors[`items.${index}.unitPrice`])}
              helperText={errors[`items.${index}.unitPrice`]}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "decimal" }}
            />
            <TextField
              label={t("provider.proposalCreate.itemTotal")}
              value={String(itemLineTotal(item))}
              disabled
              fullWidth
              helperText={t("provider.proposalCreate.itemTotalHint")}
            />
            <TextField
              label={t("provider.proposalCreate.itemDescription")}
              value={item.description}
              onChange={(event) =>
                patchItem(index, { description: event.target.value.slice(0, 2000) })
              }
              error={Boolean(errors[`items.${index}.description`])}
              helperText={
                errors[`items.${index}.description`] ?? `${item.description.length}/2000`
              }
              disabled={submitting}
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
        disabled={submitting}
        onClick={() => patch({ items: [...values.items, createEmptyItem(requestType)] })}
        sx={{ alignSelf: "flex-start" }}
      >
        {t("provider.proposalCreate.addItem")}
      </Button>

      <Button type="submit" variant="contained" disabled={submitting}>
        {submitting
          ? t("provider.proposalCreate.submitting")
          : t("provider.proposalCreate.submit")}
      </Button>
    </Stack>
  );
}
