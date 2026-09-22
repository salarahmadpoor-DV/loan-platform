import { Stack, Typography } from "@mui/material";
import type { ReactNode } from "react";
import { t } from "../../../../../shared/i18n";
import { AttributeFields } from "./AttributeFields";
import { CatalogPicker } from "./CatalogPicker";
import {
  useCatalogServiceDetail,
  useCatalogServices,
  useServiceCategories,
} from "../hooks/useCatalogServices";

type ServiceCatalogSectionProps = {
  categoryId: string;
  serviceId: string;
  attributes: Record<string, string>;
  onCategoryChange: (categoryId: string) => void;
  onServiceChange: (serviceId: string) => void;
  onAttributeChange: (attributeId: string, value: string) => void;
  serviceError?: string;
  attributeErrors?: Record<string, string>;
  disabled?: boolean;
  fieldLabel: (label: string, required: boolean) => ReactNode;
};

export function ServiceCatalogSection({
  categoryId,
  serviceId,
  attributes,
  onCategoryChange,
  onServiceChange,
  onAttributeChange,
  serviceError,
  attributeErrors,
  disabled,
  fieldLabel,
}: ServiceCatalogSectionProps) {
  const categories = useServiceCategories();
  const categoryItems = categories.data ?? [];
  const selectedCategory = Number.parseInt(categoryId, 10);
  const services = useCatalogServices(
    Number.isFinite(selectedCategory) && selectedCategory > 0 ? selectedCategory : undefined,
  );
  const serviceItems = services.data?.items ?? [];
  const selectedService = Number.parseInt(serviceId, 10);
  const detail = useCatalogServiceDetail(
    Number.isFinite(selectedService) && selectedService > 0 ? selectedService : undefined,
  );

  if (categories.isError || (!categories.isPending && categoryItems.length === 0)) {
    return (
      <Typography variant="body2" color="text.secondary">
        {t("request.create.catalogUnavailable")}
      </Typography>
    );
  }

  return (
    <Stack spacing={1.5}>
      <CatalogPicker
        options={categoryItems}
        valueId={categoryId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={onCategoryChange}
        label={fieldLabel(t("request.create.serviceCategory"), true)}
        disabled={disabled}
        required
        loading={categories.isPending}
      />
      <CatalogPicker
        options={serviceItems}
        valueId={serviceId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={onServiceChange}
        label={fieldLabel(t("request.create.serviceSelect"), true)}
        error={serviceError}
        disabled={disabled || !categoryId}
        required
        loading={services.isPending}
      />
      <AttributeFields
        attributes={detail.data?.attributes ?? []}
        values={attributes}
        onChange={onAttributeChange}
        errors={attributeErrors}
        disabled={disabled}
      />
    </Stack>
  );
}
