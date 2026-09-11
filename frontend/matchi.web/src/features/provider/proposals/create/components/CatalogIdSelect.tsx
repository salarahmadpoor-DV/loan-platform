import { Autocomplete, TextField, Typography } from "@mui/material";
import { t } from "../../../../../shared/i18n";

type CatalogOption = {
  id: number;
  label: string;
};

type CatalogIdSelectProps = {
  label: string;
  value: string;
  options: CatalogOption[];
  loading: boolean;
  catalogFailed: boolean;
  error?: string;
  disabled?: boolean;
  onChange: (id: string) => void;
};

export function CatalogIdSelect({
  label,
  value,
  options,
  loading,
  catalogFailed,
  error,
  disabled,
  onChange,
}: CatalogIdSelectProps) {
  const selected = options.find((option) => String(option.id) === value) ?? null;

  if (catalogFailed || (!loading && options.length === 0)) {
    return (
      <>
        <Typography variant="body2" color="text.secondary">
          {t("provider.proposalCreate.catalogFallback")}
        </Typography>
        <TextField
          label={label}
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
    <Autocomplete<CatalogOption>
      options={options}
      loading={loading}
      value={selected}
      disabled={disabled}
      getOptionLabel={(option) => option.label}
      isOptionEqualToValue={(option, optionValue) => option.id === optionValue.id}
      onChange={(_event, next) => onChange(next ? String(next.id) : "")}
      renderInput={(params) => (
        <TextField {...params} label={label} error={Boolean(error)} helperText={error} />
      )}
    />
  );
}
