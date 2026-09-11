import { Autocomplete, TextField, Typography } from "@mui/material";
import { t } from "../../../../../shared/i18n";
import type { CatalogService } from "../api/createRequestTypes";
import { useCatalogServices } from "../hooks/useCatalogServices";

type ServiceSelectProps = {
  value: string;
  onChange: (serviceId: string) => void;
  error?: string;
  disabled?: boolean;
};

export function ServiceSelect({ value, onChange, error, disabled }: ServiceSelectProps) {
  const catalog = useCatalogServices();
  const items = catalog.data?.items ?? [];
  const selected = items.find((item) => String(item.id) === value) ?? null;

  if (catalog.isError || (!catalog.isPending && items.length === 0)) {
    return (
      <>
        <Typography variant="body2" color="text.secondary">
          {t("request.create.catalogUnavailable")}
        </Typography>
        <TextField
          label={t("request.create.serviceIdFallback")}
          value={value}
          onChange={(event) => onChange(event.target.value)}
          error={Boolean(error)}
          helperText={error}
          disabled={disabled}
          fullWidth
          inputProps={{ inputMode: "numeric" }}
        />
      </>
    );
  }

  return (
    <Autocomplete<CatalogService>
      options={items}
      loading={catalog.isPending}
      value={selected}
      disabled={disabled}
      getOptionLabel={(option) => option.name}
      isOptionEqualToValue={(option, optionValue) => option.id === optionValue.id}
      onChange={(_event, next) => onChange(next ? String(next.id) : "")}
      renderInput={(params) => (
        <TextField
          {...params}
          label={t("request.create.serviceSelect")}
          error={Boolean(error)}
          helperText={error}
        />
      )}
    />
  );
}
