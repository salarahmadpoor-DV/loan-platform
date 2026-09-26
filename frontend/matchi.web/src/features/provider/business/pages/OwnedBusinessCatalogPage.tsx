import {
  Alert,
  Button,
  Checkbox,
  FormControlLabel,
  Stack,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { ProductCatalogAdd, ServiceCatalogAdd } from "../../offerings/components/CatalogAddPickers";
import { OwnedBusinessPage } from "../components/OwnedBusinessPage";
import {
  useOwnedBusinessCatalogMutations,
  useOwnedBusinessProducts,
  useOwnedBusinessServices,
} from "../hooks/useOwnedBusinessCatalog";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";

function parsePrice(raw: string): number | null {
  const trimmed = raw.trim();
  if (!trimmed) {
    return null;
  }
  const value = Number(trimmed);
  return Number.isFinite(value) && value >= 0 ? value : null;
}

function CatalogBody({ business }: { business: MyBusinessProfile }) {
  const services = useOwnedBusinessServices(business.id);
  const products = useOwnedBusinessProducts(business.id);
  const mutations = useOwnedBusinessCatalogMutations(business.id);
  const [message, setMessage] = useState<string | null>(null);
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");
  const [chooseProvider, setChooseProvider] = useState(false);
  const [productPrice, setProductPrice] = useState("");
  const [productAvailable, setProductAvailable] = useState(true);

  const error =
    services.error ??
    products.error ??
    mutations.addService.error ??
    mutations.updateService.error ??
    mutations.deleteService.error ??
    mutations.addProduct.error ??
    mutations.updateProduct.error ??
    mutations.deleteProduct.error;

  return (
    <Stack spacing={2}>
      {error ? <ErrorAlert error={error} /> : null}
      {message ? (
        <Alert severity="success" onClose={() => setMessage(null)}>
          {message}
        </Alert>
      ) : null}
      <AppCard>
        <Stack spacing={1.5}>
          <Typography variant="h6">{t("offerings.servicesTitle")}</Typography>
          <Typography variant="body2" color="text.secondary">
            {t("offerings.businessServicesHint")}
          </Typography>
          <TextField
            label={t("offerings.minPrice")}
            value={minPrice}
            onChange={(event) => setMinPrice(event.target.value)}
            inputProps={{ inputMode: "decimal" }}
          />
          <TextField
            label={t("offerings.maxPrice")}
            value={maxPrice}
            onChange={(event) => setMaxPrice(event.target.value)}
            inputProps={{ inputMode: "decimal" }}
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={chooseProvider}
                onChange={(event) => setChooseProvider(event.target.checked)}
              />
            }
            label={t("offerings.chooseProvider")}
          />
          <ServiceCatalogAdd
            pending={mutations.addService.isPending}
            existingIds={(services.data ?? []).map((item) => item.serviceId)}
            onAdd={(serviceId) => {
              mutations.addService.mutate(
                {
                  serviceId,
                  isActive: true,
                  canCustomerChooseProvider: chooseProvider,
                  minPrice: parsePrice(minPrice),
                  maxPrice: parsePrice(maxPrice),
                },
                { onSuccess: () => setMessage(t("offerings.serviceAdded")) },
              );
            }}
          />
          {services.isPending ? <LoadingState /> : null}
          {!services.isPending && (services.data?.length ?? 0) === 0 ? (
            <EmptyState title={t("offerings.servicesEmpty")} />
          ) : null}
          {(services.data ?? []).map((item) => (
            <Stack key={item.serviceId} spacing={1}>
              <Typography variant="subtitle1">{item.serviceName ?? t("common.notSpecified")}</Typography>
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                <StatusChip
                  label={item.isActive ? t("offerings.active") : t("offerings.inactive")}
                  tone={item.isActive ? "success" : "neutral"}
                />
                {item.canCustomerChooseProvider ? (
                  <StatusChip label={t("offerings.chooseProvider")} tone="info" />
                ) : null}
              </Stack>
              <Typography variant="body2" color="text.secondary">
                {t("offerings.priceRange", {
                  min: item.minPrice ?? "—",
                  max: item.maxPrice ?? "—",
                })}
              </Typography>
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                <Button
                  variant="outlined"
                  disabled={mutations.updateService.isPending}
                  onClick={() =>
                    mutations.updateService.mutate(
                      {
                        serviceId: item.serviceId,
                        isActive: !item.isActive,
                        canCustomerChooseProvider: item.canCustomerChooseProvider,
                        minPrice: item.minPrice,
                        maxPrice: item.maxPrice,
                      },
                      { onSuccess: () => setMessage(t("offerings.updated")) },
                    )
                  }
                >
                  {item.isActive ? t("offerings.deactivate") : t("offerings.activate")}
                </Button>
                <Button
                  color="error"
                  variant="outlined"
                  disabled={mutations.deleteService.isPending}
                  onClick={() =>
                    mutations.deleteService.mutate(item.serviceId, {
                      onSuccess: () => setMessage(t("offerings.removed")),
                    })
                  }
                >
                  {t("offerings.remove")}
                </Button>
              </Stack>
            </Stack>
          ))}
        </Stack>
      </AppCard>
      <AppCard>
        <Stack spacing={1.5}>
          <Typography variant="h6">{t("offerings.productsTitle")}</Typography>
          <TextField
            label={t("offerings.price")}
            value={productPrice}
            onChange={(event) => setProductPrice(event.target.value)}
            inputProps={{ inputMode: "decimal" }}
          />
          <FormControlLabel
            control={
              <Switch
                checked={productAvailable}
                onChange={(event) => setProductAvailable(event.target.checked)}
              />
            }
            label={t("offerings.available")}
          />
          <ProductCatalogAdd
            pending={mutations.addProduct.isPending}
            existingIds={(products.data ?? []).map((item) => item.productId)}
            onAdd={(product) => {
              mutations.addProduct.mutate(
                {
                  productId: product.id,
                  price: parsePrice(productPrice),
                  isAvailable: productAvailable,
                },
                { onSuccess: () => setMessage(t("offerings.productAdded")) },
              );
            }}
          />
          {products.isPending ? <LoadingState /> : null}
          {!products.isPending && (products.data?.length ?? 0) === 0 ? (
            <EmptyState title={t("offerings.productsEmpty")} />
          ) : null}
          {(products.data ?? []).map((item) => (
            <Stack key={item.productId} spacing={1}>
              <Typography variant="subtitle1">{item.productName ?? t("common.notSpecified")}</Typography>
              <Typography variant="body2" color="text.secondary">
                {item.price != null ? t("offerings.priceValue", { price: item.price }) : t("offerings.noPrice")}
              </Typography>
              <StatusChip
                label={item.isAvailable ? t("offerings.available") : t("offerings.unavailable")}
                tone={item.isAvailable ? "success" : "neutral"}
              />
              <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                <Button
                  variant="outlined"
                  disabled={mutations.updateProduct.isPending}
                  onClick={() =>
                    mutations.updateProduct.mutate(
                      {
                        productId: item.productId,
                        price: item.price,
                        isAvailable: !item.isAvailable,
                        minOrderQuantity: item.minOrderQuantity,
                        leadTimeDays: item.leadTimeDays,
                      },
                      { onSuccess: () => setMessage(t("offerings.updated")) },
                    )
                  }
                >
                  {item.isAvailable ? t("offerings.markUnavailable") : t("offerings.markAvailable")}
                </Button>
                <Button
                  color="error"
                  variant="outlined"
                  disabled={mutations.deleteProduct.isPending}
                  onClick={() =>
                    mutations.deleteProduct.mutate(item.productId, {
                      onSuccess: () => setMessage(t("offerings.removed")),
                    })
                  }
                >
                  {t("offerings.remove")}
                </Button>
              </Stack>
            </Stack>
          ))}
        </Stack>
      </AppCard>
    </Stack>
  );
}

export function OwnedBusinessCatalogPage() {
  return (
    <OwnedBusinessPage title={t("offerings.businessTitle")} description={t("offerings.businessDescription")}>
      {(business) => <CatalogBody business={business} />}
    </OwnedBusinessPage>
  );
}
