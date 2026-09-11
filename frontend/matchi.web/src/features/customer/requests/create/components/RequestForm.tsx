import { Button, Stack, TextField, Typography } from "@mui/material";
import { useState, type FormEvent } from "react";
import { t } from "../../../../../shared/i18n";
import type { RequestKind } from "../../../../../shared/types/marketplace";
import { AppCard } from "../../../../../shared/ui/AppCard";
import type { CreateRequestBody } from "../api/createRequestTypes";
import {
  defaultCreateRequestValues,
  toCreateRequestBody,
  validateCreateRequestForm,
  type CreateRequestFieldErrors,
  type CreateRequestFormValues,
} from "../model/createRequestForm";
import { RequestKindSelector } from "./RequestKindSelector";
import { ServiceSelect } from "./ServiceSelect";

type RequestFormProps = {
  submitting: boolean;
  onSubmit: (body: CreateRequestBody) => void;
};

export function RequestForm({ submitting, onSubmit }: RequestFormProps) {
  const [values, setValues] = useState<CreateRequestFormValues>(defaultCreateRequestValues);
  const [errors, setErrors] = useState<CreateRequestFieldErrors>({});

  const showService = values.requestType === "Service" || values.requestType === "Hybrid";
  const showProduct = values.requestType === "Product" || values.requestType === "Hybrid";

  function patch(update: Partial<CreateRequestFormValues>) {
    setValues((current) => ({ ...current, ...update }));
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const nextErrors = validateCreateRequestForm(values);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }
    onSubmit(toCreateRequestBody(values));
  }

  return (
    <Stack component="form" spacing={2} onSubmit={handleSubmit} noValidate>
      <TextField
        label={t("request.create.title")}
        value={values.title}
        onChange={(event) => patch({ title: event.target.value })}
        required
        fullWidth
        error={Boolean(errors.title)}
        helperText={errors.title}
        disabled={submitting}
      />
      <TextField
        label={t("request.create.description")}
        value={values.description}
        onChange={(event) => patch({ description: event.target.value })}
        fullWidth
        multiline
        minRows={3}
        disabled={submitting}
      />

      <Typography variant="subtitle2">{t("request.create.kind")}</Typography>
      <RequestKindSelector
        value={values.requestType}
        disabled={submitting}
        onChange={(requestType: RequestKind) => patch({ requestType })}
      />
      {errors.requestType ? (
        <Typography color="error" variant="caption">
          {errors.requestType}
        </Typography>
      ) : null}

      {showService ? (
        <AppCard>
          <Stack spacing={2}>
            <Typography variant="subtitle1">{t("request.create.serviceSection")}</Typography>
            <Typography variant="body2" color="text.secondary">
              {t("request.create.serviceHint")}
            </Typography>
            <ServiceSelect
              value={values.serviceId}
              onChange={(serviceId) => patch({ serviceId })}
              error={errors.serviceId}
              disabled={submitting}
            />
            <TextField
              label={t("request.create.serviceQuantity")}
              value={values.serviceQuantity}
              onChange={(event) => patch({ serviceQuantity: event.target.value })}
              error={Boolean(errors.serviceQuantity)}
              helperText={errors.serviceQuantity}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "decimal" }}
            />
            <TextField
              label={t("request.create.serviceLineDescription")}
              value={values.serviceDescription}
              onChange={(event) => patch({ serviceDescription: event.target.value })}
              disabled={submitting}
              fullWidth
            />
          </Stack>
        </AppCard>
      ) : null}

      {showProduct ? (
        <AppCard>
          <Stack spacing={2}>
            <Typography variant="subtitle1">{t("request.create.productSection")}</Typography>
            <Typography variant="body2" color="text.secondary">
              {t("request.create.productHint")}
            </Typography>
            <TextField
              label={t("request.create.productId")}
              value={values.productId}
              onChange={(event) => patch({ productId: event.target.value })}
              error={Boolean(errors.productId)}
              helperText={errors.productId}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "numeric" }}
            />
            <TextField
              label={t("request.create.productCategoryId")}
              value={values.productCategoryId}
              onChange={(event) => patch({ productCategoryId: event.target.value })}
              error={Boolean(errors.productCategoryId)}
              helperText={errors.productCategoryId}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "numeric" }}
            />
            <TextField
              label={t("request.create.productQuantity")}
              value={values.productQuantity}
              onChange={(event) => patch({ productQuantity: event.target.value })}
              error={Boolean(errors.productQuantity)}
              helperText={errors.productQuantity}
              disabled={submitting}
              fullWidth
              inputProps={{ inputMode: "decimal" }}
            />
            <TextField
              label={t("request.create.productUnit")}
              value={values.productUnit}
              onChange={(event) => patch({ productUnit: event.target.value })}
              disabled={submitting}
              fullWidth
            />
            <TextField
              label={t("request.create.productLineDescription")}
              value={values.productDescription}
              onChange={(event) => patch({ productDescription: event.target.value })}
              disabled={submitting}
              fullWidth
            />
          </Stack>
        </AppCard>
      ) : null}

      <Button type="submit" variant="contained" size="large" disabled={submitting} fullWidth>
        {submitting ? t("request.create.submitting") : t("request.create.submit")}
      </Button>
    </Stack>
  );
}
