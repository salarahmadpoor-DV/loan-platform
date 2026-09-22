import { Box, Button, Stack, TextField, Typography } from "@mui/material";
import { useState, type FormEvent, type ReactNode } from "react";
import { Link as RouterLink } from "react-router-dom";
import { t, type MessageKey } from "../../../../../shared/i18n";
import type { RequestKind } from "../../../../../shared/types/marketplace";
import { AppCard } from "../../../../../shared/ui/AppCard";
import { FormSplitLayout } from "../../../../../shared/ui/FormSplitLayout";
import { MarketplaceStepper } from "../../../../../shared/ui/MarketplaceStepper";
import type { CreateRequestBody } from "../api/createRequestTypes";
import {
  useCatalogProducts,
  useCatalogServices,
  useProductCategories,
  useServiceCategories,
} from "../hooks/useCatalogServices";
import {
  defaultCreateRequestValues,
  firstRequestErrorStep,
  requestStepFieldErrors,
  toCreateRequestBody,
  validateCreateRequestForm,
  type CreateRequestFieldErrors,
  type CreateRequestFormValues,
} from "../model/createRequestForm";
import { RequestKindSelector } from "./RequestKindSelector";
import { RequestLocationMapPicker, RequestPlaceFields } from "./RequestLocationMapPicker";
import { ProductCatalogSection } from "./ProductCatalogSection";
import { ServiceCatalogSection } from "./ServiceSelect";

const STEP_KEYS: MessageKey[] = [
  "request.create.step.need",
  "request.create.step.type",
  "request.create.step.describe",
  "request.create.step.location",
  "request.create.step.details",
  "request.create.step.review",
];

const KIND_LABEL: Record<RequestKind, MessageKey> = {
  Service: "request.kind.Service",
  Product: "request.kind.Product",
  Hybrid: "request.kind.Hybrid",
};

type RequestFormProps = {
  submitting: boolean;
  onSubmit: (body: CreateRequestBody) => void;
  initialTitle?: string;
};

function dash(value: string): string {
  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : t("common.notSpecified");
}

function fieldLabel(label: string, required: boolean): ReactNode {
  return (
    <>
      {label}
      <Typography
        component="span"
        variant="caption"
        color="text.secondary"
        sx={{ marginInlineStart: 0.75 }}
      >
        ({required ? t("request.create.required") : t("request.create.optional")})
      </Typography>
    </>
  );
}

function SummaryRow({ label, value }: { label: string; value: string }) {
  return (
    <Stack spacing={0.25}>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body2">{value}</Typography>
    </Stack>
  );
}

export function RequestForm({ submitting, onSubmit, initialTitle }: RequestFormProps) {
  const [values, setValues] = useState<CreateRequestFormValues>({
    ...defaultCreateRequestValues,
    title: initialTitle ?? "",
  });
  const [errors, setErrors] = useState<CreateRequestFieldErrors>({});
  const [step, setStep] = useState(0);
  const [unlocked, setUnlocked] = useState(0);
  const catalog = useCatalogServices(
    values.serviceCategoryId ? Number.parseInt(values.serviceCategoryId, 10) : undefined,
  );
  const serviceCategories = useServiceCategories();
  const productCategories = useProductCategories();
  const products = useCatalogProducts(
    values.productCategoryId ? Number.parseInt(values.productCategoryId, 10) : undefined,
  );

  const showService = values.requestType === "Service" || values.requestType === "Hybrid";
  const showProduct = values.requestType === "Product" || values.requestType === "Hybrid";
  const lastStep = STEP_KEYS.length - 1;
  const serviceName =
    catalog.data?.items.find((item) => String(item.id) === values.serviceId)?.name ??
    serviceCategories.data?.find((item) => String(item.id) === values.serviceCategoryId)?.name ??
    values.serviceId;
  const productName =
    products.data?.items.find((item) => String(item.id) === values.productId)?.name ??
    values.productId;
  const productCategoryName =
    productCategories.data?.find((item) => String(item.id) === values.productCategoryId)?.name ??
    values.productCategoryId;

  function patch(update: Partial<CreateRequestFormValues>) {
    setValues((current) => ({ ...current, ...update }));
  }

  function goToStep(index: number) {
    if (index <= unlocked) {
      setStep(index);
    }
  }

  function goBack() {
    setStep((current) => Math.max(0, current - 1));
  }

  function goNext() {
    const nextErrors = requestStepFieldErrors(step, values);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }
    const next = Math.min(step + 1, lastStep);
    setStep(next);
    setUnlocked((current) => Math.max(current, next));
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (submitting) {
      return;
    }
    if (step < lastStep) {
      goNext();
      return;
    }
    const nextErrors = validateCreateRequestForm(values);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      const errorStep = firstRequestErrorStep(values);
      setStep(errorStep);
      setUnlocked((current) => Math.max(current, errorStep));
      return;
    }
    onSubmit(toCreateRequestBody(values));
  }

  const summary = (
    <AppCard>
      <Stack spacing={1.5}>
        <Typography variant="h6">{t("request.create.summaryTitle")}</Typography>
        <SummaryRow label={t("request.create.title")} value={dash(values.title)} />
        <SummaryRow label={t("request.create.kind")} value={t(KIND_LABEL[values.requestType])} />
        {showService ? (
          <SummaryRow label={t("request.create.serviceSelect")} value={dash(serviceName)} />
        ) : null}
        {showProduct ? (
          <>
            <SummaryRow
              label={t("request.create.productCategory")}
              value={dash(productCategoryName)}
            />
            <SummaryRow label={t("request.create.productSelect")} value={dash(productName)} />
          </>
        ) : null}
        <SummaryRow
          label={t("request.create.mapTitle")}
          value={
            values.mapPoint
              ? t("request.create.mapSelected")
              : t("request.create.mapNotSelected")
          }
        />
        <Typography variant="caption" color="text.secondary">
          {t("request.create.stepProgress", { current: step + 1, total: STEP_KEYS.length })}
        </Typography>
      </Stack>
    </AppCard>
  );

  return (
    <Stack component="form" spacing={2} onSubmit={handleSubmit} noValidate sx={{ minWidth: 0 }}>
      <MarketplaceStepper
        steps={STEP_KEYS.map((key) => t(key))}
        activeStep={step}
        maxUnlocked={unlocked}
        onStep={goToStep}
        progressLabel={t("request.create.stepProgress", {
          current: step + 1,
          total: STEP_KEYS.length,
        })}
      />
      <FormSplitLayout
        hideSummaryOnMobile
        main={
          <AppCard>
            <Stack spacing={2}>
              {step === 0 ? (
                <>
                  <Typography variant="h6">{t("request.create.step.need")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {t("request.create.needHint")}
                  </Typography>
                  <TextField
                    label={fieldLabel(t("request.create.title"), true)}
                    value={values.title}
                    onChange={(event) => patch({ title: event.target.value })}
                    required
                    fullWidth
                    error={Boolean(errors.title)}
                    helperText={errors.title}
                    disabled={submitting}
                    autoFocus
                  />
                </>
              ) : null}

              {step === 1 ? (
                <>
                  <Typography variant="h6">{t("request.create.step.type")}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    {t("request.create.required")}
                  </Typography>
                  <RequestKindSelector
                    value={values.requestType}
                    disabled={submitting}
                    error={errors.requestType}
                    onChange={(requestType: RequestKind) => patch({ requestType })}
                  />
                </>
              ) : null}

              {step === 2 ? (
                <>
                  <Typography variant="h6">{t("request.create.step.describe")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {t("request.create.describeHint")}
                  </Typography>
                  <TextField
                    label={fieldLabel(t("request.create.description"), false)}
                    value={values.description}
                    onChange={(event) => patch({ description: event.target.value })}
                    fullWidth
                    multiline
                    minRows={4}
                    disabled={submitting}
                  />
                  {showService ? (
                    <Stack spacing={1}>
                      <Typography variant="subtitle2">{t("request.create.serviceSection")}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {t("request.create.serviceHint")}
                      </Typography>
                      <ServiceCatalogSection
                        categoryId={values.serviceCategoryId}
                        serviceId={values.serviceId}
                        attributes={values.serviceAttributes}
                        onCategoryChange={(serviceCategoryId) =>
                          patch({
                            serviceCategoryId,
                            serviceId: "",
                            serviceAttributes: {},
                          })
                        }
                        onServiceChange={(serviceId) =>
                          patch({ serviceId, serviceAttributes: {} })
                        }
                        onAttributeChange={(attributeId, value) =>
                          patch({
                            serviceAttributes: {
                              ...values.serviceAttributes,
                              [attributeId]: value,
                            },
                          })
                        }
                        serviceError={errors.serviceId}
                        disabled={submitting}
                        fieldLabel={fieldLabel}
                      />
                    </Stack>
                  ) : null}
                  {showProduct ? (
                    <Stack spacing={1}>
                      <Typography variant="subtitle2">{t("request.create.productSection")}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {t("request.create.productHint")}
                      </Typography>
                      <ProductCatalogSection
                        categoryId={values.productCategoryId}
                        productId={values.productId}
                        attributes={values.productAttributes}
                        onCategoryChange={(productCategoryId) =>
                          patch({
                            productCategoryId,
                            productId: "",
                            productAttributes: {},
                          })
                        }
                        onProductChange={(productId, product) =>
                          patch({
                            productId,
                            productCategoryId: product
                              ? String(product.categoryId)
                              : values.productCategoryId,
                            productAttributes: {},
                          })
                        }
                        onAttributeChange={(attributeId, value) =>
                          patch({
                            productAttributes: {
                              ...values.productAttributes,
                              [attributeId]: value,
                            },
                          })
                        }
                        productError={errors.productId ?? errors.productCategoryId}
                        disabled={submitting}
                        fieldLabel={fieldLabel}
                      />
                    </Stack>
                  ) : null}
                </>
              ) : null}

              {step === 3 ? (
                <>
                  <Typography variant="h6">{t("request.create.step.location")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {t("request.create.locationNote")}
                  </Typography>
                  <RequestLocationMapPicker
                    value={values.mapPoint}
                    onChange={(mapPoint) => patch({ mapPoint })}
                    onResolved={(location) => patch({ location })}
                    error={errors.mapPoint}
                  />
                  <RequestPlaceFields
                    location={values.location}
                    address={values.address}
                    errors={{
                      provinceId: errors.provinceId,
                      cityId: errors.cityId,
                      districtId: errors.districtId,
                      address: errors.address,
                    }}
                    disabled={submitting}
                    onLocationChange={(location) => patch({ location })}
                    onAddressChange={(address) => patch({ address })}
                  />
                </>
              ) : null}

              {step === 4 ? (
                <>
                  <Typography variant="h6">{t("request.create.step.details")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {t("request.create.detailsHint")}
                  </Typography>
                  {showService ? (
                    <>
                      <TextField
                        label={fieldLabel(t("request.create.serviceQuantity"), true)}
                        value={values.serviceQuantity}
                        onChange={(event) => patch({ serviceQuantity: event.target.value })}
                        error={Boolean(errors.serviceQuantity)}
                        helperText={errors.serviceQuantity}
                        disabled={submitting}
                        required
                        fullWidth
                        inputProps={{ inputMode: "decimal" }}
                      />
                      <TextField
                        label={fieldLabel(t("request.create.serviceLineDescription"), false)}
                        value={values.serviceDescription}
                        onChange={(event) => patch({ serviceDescription: event.target.value })}
                        disabled={submitting}
                        fullWidth
                      />
                    </>
                  ) : null}
                  {showProduct ? (
                    <>
                      <TextField
                        label={fieldLabel(t("request.create.productQuantity"), true)}
                        value={values.productQuantity}
                        onChange={(event) => patch({ productQuantity: event.target.value })}
                        error={Boolean(errors.productQuantity)}
                        helperText={errors.productQuantity}
                        disabled={submitting}
                        required
                        fullWidth
                        inputProps={{ inputMode: "decimal" }}
                      />
                      <TextField
                        label={fieldLabel(t("request.create.productUnit"), false)}
                        value={values.productUnit}
                        onChange={(event) => patch({ productUnit: event.target.value })}
                        disabled={submitting}
                        fullWidth
                      />
                      <TextField
                        label={fieldLabel(t("request.create.productLineDescription"), false)}
                        value={values.productDescription}
                        onChange={(event) => patch({ productDescription: event.target.value })}
                        disabled={submitting}
                        fullWidth
                      />
                    </>
                  ) : null}
                </>
              ) : null}

              {step === 5 ? (
                <>
                  <Typography variant="h6">{t("request.create.reviewTitle")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {t("request.create.reviewHint")}
                  </Typography>
                  <SummaryRow label={t("request.create.title")} value={dash(values.title)} />
                  <SummaryRow
                    label={t("request.create.description")}
                    value={dash(values.description)}
                  />
                  <SummaryRow
                    label={t("request.create.kind")}
                    value={t(KIND_LABEL[values.requestType])}
                  />
                  {showService ? (
                    <>
                      <SummaryRow
                        label={t("request.create.serviceSelect")}
                        value={dash(serviceName)}
                      />
                      <SummaryRow
                        label={t("request.create.serviceQuantity")}
                        value={dash(values.serviceQuantity)}
                      />
                      <SummaryRow
                        label={t("request.create.serviceLineDescription")}
                        value={dash(values.serviceDescription)}
                      />
                    </>
                  ) : null}
                  {showProduct ? (
                    <>
                      <SummaryRow
                        label={t("request.create.productCategory")}
                        value={dash(productCategoryName)}
                      />
                      <SummaryRow
                        label={t("request.create.productSelect")}
                        value={dash(productName)}
                      />
                      <SummaryRow
                        label={t("request.create.productQuantity")}
                        value={dash(values.productQuantity)}
                      />
                      <SummaryRow
                        label={t("request.create.productUnit")}
                        value={dash(values.productUnit)}
                      />
                    </>
                  ) : null}
                  <SummaryRow
                    label={t("request.create.mapTitle")}
                    value={
                      values.mapPoint
                        ? t("request.create.mapSelected")
                        : t("request.create.mapNotSelected")
                    }
                  />
                  <SummaryRow
                    label={t("request.create.province")}
                    value={dash(values.location.provinceName ?? "")}
                  />
                  <SummaryRow
                    label={t("request.create.city")}
                    value={dash(values.location.cityName ?? "")}
                  />
                  <SummaryRow
                    label={t("request.create.district")}
                    value={dash(values.location.districtName ?? "")}
                  />
                  <SummaryRow label={t("request.create.address")} value={dash(values.address)} />
                </>
              ) : null}

              <Box
                sx={{
                  display: "flex",
                  flexDirection: { xs: "column-reverse", sm: "row" },
                  gap: 1,
                  justifyContent: "space-between",
                  position: { xs: "sticky", md: "static" },
                  bottom: 0,
                  bgcolor: "background.paper",
                  pt: 1,
                  zIndex: 1,
                }}
              >
                {step === 0 ? (
                  <Button
                    component={RouterLink}
                    to="/customer/requests"
                    variant="outlined"
                    sx={{ minHeight: 48, width: { xs: "100%", sm: "auto" } }}
                  >
                    {t("request.create.cancel")}
                  </Button>
                ) : (
                  <Button
                    type="button"
                    variant="outlined"
                    onClick={goBack}
                    disabled={submitting}
                    sx={{ minHeight: 48, width: { xs: "100%", sm: "auto" } }}
                  >
                    {t("request.create.back")}
                  </Button>
                )}
                {step < lastStep ? (
                  <Button
                    type="submit"
                    variant="contained"
                    sx={{ minHeight: 48, width: { xs: "100%", sm: "auto" } }}
                    disabled={submitting}
                  >
                    {t("request.create.next")}
                  </Button>
                ) : (
                  <Button
                    type="submit"
                    variant="contained"
                    size="large"
                    disabled={submitting}
                    sx={{ minHeight: 48, width: { xs: "100%", sm: "auto" } }}
                  >
                    {submitting ? t("request.create.submitting") : t("request.create.submit")}
                  </Button>
                )}
              </Box>
            </Stack>
          </AppCard>
        }
        summary={summary}
      />
    </Stack>
  );
}
