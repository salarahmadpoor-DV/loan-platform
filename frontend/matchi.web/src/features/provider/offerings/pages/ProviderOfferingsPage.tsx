import {
  Alert,
  Button,
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
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { ProductCatalogAdd, ServiceCatalogAdd } from "../components/CatalogAddPickers";
import {
  useMyProviderProducts,
  useMyProviderServices,
  useProviderOfferingMutations,
} from "../hooks/useProviderOfferingMutations";

function parsePrice(raw: string): number | null {
  const trimmed = raw.trim();
  if (!trimmed) {
    return null;
  }
  const value = Number(trimmed);
  return Number.isFinite(value) && value >= 0 ? value : null;
}

export function ProviderOfferingsPage() {
  const services = useMyProviderServices();
  const products = useMyProviderProducts();
  const mutations = useProviderOfferingMutations();
  const [message, setMessage] = useState<string | null>(null);
  const [productPrice, setProductPrice] = useState("");
  const [productAvailable, setProductAvailable] = useState(true);

  const error =
    services.error ??
    products.error ??
    mutations.addService.error ??
    mutations.deleteService.error ??
    mutations.updateService.error ??
    mutations.addProduct.error ??
    mutations.updateProduct.error ??
    mutations.deleteProduct.error;

  return (
    <>
      <PageHeader title={t("offerings.providerTitle")} description={t("offerings.providerDescription")} />
      {error ? <ErrorAlert error={error} /> : null}
      {message ? (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMessage(null)}>
          {message}
        </Alert>
      ) : null}

      <Stack spacing={2}>
        <AppCard>
          <Stack spacing={1.5}>
            <Typography variant="h6">{t("offerings.servicesTitle")}</Typography>
            <Typography variant="body2" color="text.secondary">
              {t("offerings.servicesHint")}
            </Typography>
            <ServiceCatalogAdd
              pending={mutations.addService.isPending}
              existingIds={(services.data ?? []).map((item) => item.serviceId)}
              onAdd={(serviceId) => {
                mutations.addService.mutate(serviceId, {
                  onSuccess: () => setMessage(t("offerings.serviceAdded")),
                });
              }}
            />
            {services.isPending ? <LoadingState /> : null}
            {!services.isPending && (services.data?.length ?? 0) === 0 ? (
              <EmptyState title={t("offerings.servicesEmpty")} />
            ) : null}
            {(services.data ?? []).map((item) => (
              <Stack
                key={item.serviceId}
                direction={{ xs: "column", sm: "row" }}
                spacing={1}
                alignItems={{ sm: "center" }}
                justifyContent="space-between"
              >
                <Stack spacing={0.25}>
                  <Typography variant="subtitle1">{item.serviceName ?? t("common.notSpecified")}</Typography>
                  <StatusChip
                    label={item.isActive ? t("offerings.active") : t("offerings.inactive")}
                    tone={item.isActive ? "success" : "neutral"}
                  />
                </Stack>
                <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                  <Button
                    variant="outlined"
                    disabled={mutations.updateService.isPending}
                    onClick={() =>
                      mutations.updateService.mutate(
                        { serviceId: item.serviceId, isActive: !item.isActive },
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
            <Typography variant="body2" color="text.secondary">
              {t("offerings.productsHint")}
            </Typography>
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
              <Stack
                key={item.productId}
                direction={{ xs: "column", sm: "row" }}
                spacing={1}
                alignItems={{ sm: "center" }}
                justifyContent="space-between"
              >
                <Stack spacing={0.25}>
                  <Typography variant="subtitle1">{item.productName ?? t("common.notSpecified")}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {item.price != null ? t("offerings.priceValue", { price: item.price }) : t("offerings.noPrice")}
                  </Typography>
                  <StatusChip
                    label={item.isAvailable ? t("offerings.available") : t("offerings.unavailable")}
                    tone={item.isAvailable ? "success" : "neutral"}
                  />
                </Stack>
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
    </>
  );
}
