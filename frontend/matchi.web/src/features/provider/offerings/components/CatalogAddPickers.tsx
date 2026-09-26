import { Button, Stack } from "@mui/material";
import { useState } from "react";
import { CatalogPicker } from "../../../customer/requests/create/components/CatalogPicker";
import {
  useCatalogProducts,
  useCatalogServices,
  useProductCategories,
  useServiceCategories,
} from "../../../customer/requests/create/hooks/useCatalogServices";
import { t } from "../../../../shared/i18n";
import type { CatalogCategory, CatalogProduct, CatalogService } from "../../../customer/requests/create/api/createRequestTypes";

type ServicePickProps = {
  onAdd: (serviceId: number) => void;
  pending: boolean;
  existingIds: number[];
};

export function ServiceCatalogAdd({ onAdd, pending, existingIds }: ServicePickProps) {
  const [categoryId, setCategoryId] = useState("");
  const [serviceId, setServiceId] = useState("");
  const categories = useServiceCategories();
  const selectedCategory = Number.parseInt(categoryId, 10);
  const services = useCatalogServices(
    Number.isFinite(selectedCategory) && selectedCategory > 0 ? selectedCategory : undefined,
  );
  const items = (services.data?.items ?? []).filter((item) => !existingIds.includes(item.id));

  return (
    <Stack spacing={1.25}>
      <CatalogPicker<CatalogCategory>
        options={categories.data ?? []}
        valueId={categoryId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={(id) => {
          setCategoryId(id);
          setServiceId("");
        }}
        label={t("offerings.serviceCategory")}
        loading={categories.isPending}
      />
      <CatalogPicker<CatalogService>
        options={items}
        valueId={serviceId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={setServiceId}
        label={t("offerings.service")}
        disabled={!categoryId}
        loading={services.isPending}
      />
      <Button
        variant="contained"
        disabled={pending || !serviceId}
        onClick={() => onAdd(Number(serviceId))}
        sx={{ minHeight: 44, alignSelf: "flex-start" }}
      >
        {t("offerings.addService")}
      </Button>
    </Stack>
  );
}

type ProductPickProps = {
  onAdd: (product: CatalogProduct) => void;
  pending: boolean;
  existingIds: number[];
};

export function ProductCatalogAdd({ onAdd, pending, existingIds }: ProductPickProps) {
  const [categoryId, setCategoryId] = useState("");
  const [productId, setProductId] = useState("");
  const categories = useProductCategories();
  const selectedCategory = Number.parseInt(categoryId, 10);
  const products = useCatalogProducts(
    Number.isFinite(selectedCategory) && selectedCategory > 0 ? selectedCategory : undefined,
  );
  const items = (products.data?.items ?? []).filter((item) => !existingIds.includes(item.id));
  const selected = items.find((item) => String(item.id) === productId) ?? null;

  return (
    <Stack spacing={1.25}>
      <CatalogPicker<CatalogCategory>
        options={categories.data ?? []}
        valueId={categoryId}
        getId={(item) => item.id}
        getLabel={(item) =>
          item.parentId
            ? `${categories.data?.find((c) => c.id === item.parentId)?.name ?? ""} · ${item.name}`.replace(/^ · /, "")
            : item.name
        }
        onChange={(id) => {
          setCategoryId(id);
          setProductId("");
        }}
        label={t("offerings.productCategory")}
        loading={categories.isPending}
      />
      <CatalogPicker<CatalogProduct>
        options={items}
        valueId={productId}
        getId={(item) => item.id}
        getLabel={(item) => item.name}
        onChange={setProductId}
        label={t("offerings.product")}
        disabled={!categoryId}
        loading={products.isPending}
      />
      <Button
        variant="contained"
        disabled={pending || !selected}
        onClick={() => {
          if (selected) {
            onAdd(selected);
          }
        }}
        sx={{ minHeight: 44, alignSelf: "flex-start" }}
      >
        {t("offerings.addProduct")}
      </Button>
    </Stack>
  );
}
