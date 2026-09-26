import { Stack, Typography } from "@mui/material";
import type { ReactNode } from "react";
import { t } from "../../../../../shared/i18n";
import type { CatalogCategory, CatalogProduct } from "../api/createRequestTypes";
import {
  useCatalogProducts,
  useProductCategories,
  useProductCategoryAttributes,
} from "../hooks/useCatalogServices";
import { AttributeFields } from "./AttributeFields";
import { CatalogPicker } from "./CatalogPicker";

type ProductCatalogSectionProps = {
  categoryId: string;
  productId: string;
  attributes: Record<string, string>;
  onCategoryChange: (categoryId: string) => void;
  onProductChange: (productId: string, product: CatalogProduct | null) => void;
  onAttributeChange: (attributeId: string, value: string) => void;
  productError?: string;
  attributeErrors?: Record<string, string>;
  disabled?: boolean;
  fieldLabel: (label: string, required: boolean) => ReactNode;
};

function categoryLabel(item: CatalogCategory, all: CatalogCategory[]): string {
  if (!item.parentId) {
    return item.name;
  }
  const parent = all.find((candidate) => candidate.id === item.parentId);
  return parent ? `${parent.name} · ${item.name}` : item.name;
}

export function ProductCatalogSection({
  categoryId,
  productId,
  attributes,
  onCategoryChange,
  onProductChange,
  onAttributeChange,
  productError,
  attributeErrors,
  disabled,
  fieldLabel,
}: ProductCatalogSectionProps) {
  const categories = useProductCategories();
  const categoryItems = categories.data ?? [];
  const selectedCategory = Number.parseInt(categoryId, 10);
  const products = useCatalogProducts(
    Number.isFinite(selectedCategory) && selectedCategory > 0 ? selectedCategory : undefined,
  );
  const productItems = products.data?.items ?? [];
  const productAttributes = useProductCategoryAttributes(
    Number.isFinite(selectedCategory) && selectedCategory > 0 ? selectedCategory : undefined,
  );

  if (categories.isError || (!categories.isPending && categoryItems.length === 0)) {
    return (
      <Typography variant="body2" color="text.secondary">
        {t("request.create.productCatalogUnavailable")}
      </Typography>
    );
  }

  return (
    <Stack spacing={1.5}>
      <CatalogPicker
        options={categoryItems}
        valueId={categoryId}
        getId={(item) => item.id}
        getLabel={(item) => categoryLabel(item, categoryItems)}
        onChange={onCategoryChange}
        label={fieldLabel(t("request.create.productCategory"), true)}
        disabled={disabled}
        required
        loading={categories.isPending}
      />
      <CatalogPicker
        options={productItems}
        valueId={productId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={(_id, product) => onProductChange(_id, product)}
        label={fieldLabel(t("request.create.productSelect"), false)}
        error={productError}
        disabled={disabled || !categoryId}
        loading={products.isPending}
      />
      <AttributeFields
        attributes={productAttributes.data ?? []}
        values={attributes}
        onChange={onAttributeChange}
        errors={attributeErrors}
        disabled={disabled}
      />
    </Stack>
  );
}
