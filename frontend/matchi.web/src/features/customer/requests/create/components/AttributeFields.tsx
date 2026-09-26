import { MenuItem, Stack, TextField } from "@mui/material";
import { t } from "../../../../../shared/i18n";
import type { CatalogAttribute } from "../api/createRequestTypes";

type AttributeFieldsProps = {
  attributes: CatalogAttribute[];
  values: Record<string, string>;
  onChange: (attributeId: string, value: string) => void;
  errors?: Record<string, string>;
  disabled?: boolean;
};

export function AttributeFields({
  attributes,
  values,
  onChange,
  errors,
  disabled,
}: AttributeFieldsProps) {
  if (attributes.length === 0) {
    return null;
  }

  return (
    <Stack spacing={1.5}>
      {attributes.map((attribute) => {
        const key = String(attribute.id);
        const value = values[key] ?? "";
        const error = errors?.[key];
        const selectOptions = attribute.options ?? [];
        const isSelect =
          selectOptions.length > 0 || attribute.dataType.toLowerCase() === "select";

        if (isSelect) {
          return (
            <TextField
              key={attribute.id}
              select
              label={attribute.name}
              value={value}
              onChange={(event) => onChange(key, event.target.value)}
              error={Boolean(error)}
              helperText={error}
              disabled={disabled}
              required={attribute.isRequired}
              fullWidth
            >
              <MenuItem value="">
                {t("request.create.attributeNone")}
              </MenuItem>
              {selectOptions.map((option) => (
                <MenuItem key={option.id} value={option.value}>
                  {option.displayName}
                </MenuItem>
              ))}
            </TextField>
          );
        }

        return (
          <TextField
            key={attribute.id}
            label={attribute.name}
            value={value}
            onChange={(event) => onChange(key, event.target.value)}
            error={Boolean(error)}
            helperText={error}
            disabled={disabled}
            required={attribute.isRequired}
            fullWidth
            inputProps={{
              inputMode: attribute.dataType.toLowerCase() === "number" ? "decimal" : "text",
            }}
          />
        );
      })}
    </Stack>
  );
}
