import { Autocomplete, TextField } from "@mui/material";
import type { ReactNode } from "react";

type CatalogPickerProps<T> = {
  options: T[];
  valueId: string;
  getId: (option: T) => number;
  getLabel: (option: T) => string;
  onChange: (id: string, option: T | null) => void;
  label: ReactNode;
  error?: string;
  disabled?: boolean;
  required?: boolean;
  loading?: boolean;
};

export function CatalogPicker<T>({
  options,
  valueId,
  getId,
  getLabel,
  onChange,
  label,
  error,
  disabled,
  required,
  loading,
}: CatalogPickerProps<T>) {
  const selected = options.find((option) => String(getId(option)) === valueId) ?? null;

  return (
    <Autocomplete<T>
      options={options}
      loading={loading}
      value={selected}
      disabled={disabled}
      getOptionLabel={(option) => getLabel(option)}
      isOptionEqualToValue={(option, optionValue) => getId(option) === getId(optionValue)}
      onChange={(_event, next) => onChange(next ? String(getId(next)) : "", next)}
      renderInput={(params) => (
        <TextField
          {...params}
          label={label}
          required={required}
          error={Boolean(error)}
          helperText={error}
        />
      )}
    />
  );
}
