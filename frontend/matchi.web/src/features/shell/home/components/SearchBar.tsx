import Autocomplete from "@mui/material/Autocomplete";
import Box from "@mui/material/Box";
import CircularProgress from "@mui/material/CircularProgress";
import TextField from "@mui/material/TextField";
import type { FormEvent, ReactNode } from "react";
import { matchiRadius, matchiShadows } from "../../../../app/designTokens";
import { t } from "../../../../shared/i18n";

type SearchBarProps = {
  id: string;
  label: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  suggestions?: string[];
  loading?: boolean;
  catalogError?: boolean;
  error?: string;
  submitSlot: ReactNode;
};

export function SearchBar({
  id,
  label,
  placeholder,
  value,
  onChange,
  onSubmit,
  suggestions = [],
  loading = false,
  catalogError = false,
  error,
  submitSlot,
}: SearchBarProps) {
  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    onSubmit();
  }

  const helper = error
    ? error
    : catalogError
      ? t("public.search.unavailable")
      : undefined;

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{
        display: "flex",
        flexDirection: { xs: "column", sm: "row" },
        gap: 1.5,
        alignItems: { sm: "flex-start" },
        p: { xs: 1.5, sm: 2 },
        border: 1,
        borderColor: error ? "error.main" : "divider",
        borderRadius: matchiRadius.md,
        bgcolor: "background.paper",
        boxShadow: matchiShadows.card,
      }}
    >
      <Autocomplete
        freeSolo
        fullWidth
        options={suggestions}
        inputValue={value}
        onInputChange={(_, next) => onChange(next)}
        onChange={(_, next) => {
          if (typeof next === "string") {
            onChange(next);
          }
        }}
        loading={loading}
        filterOptions={(options, state) => {
          const q = state.inputValue.trim().toLowerCase();
          if (!q) {
            return options.slice(0, 8);
          }
          return options.filter((option) => option.toLowerCase().includes(q)).slice(0, 8);
        }}
        loadingText={t("public.search.loading")}
        noOptionsText={t("public.search.noSuggestions")}
        renderInput={(params) => (
          <TextField
            {...params}
            id={id}
            label={label}
            placeholder={placeholder}
            error={Boolean(error)}
            helperText={helper}
            inputProps={{
              ...params.inputProps,
              autoComplete: "off",
            }}
            InputProps={{
              ...params.InputProps,
              endAdornment: (
                <>
                  {loading ? <CircularProgress color="inherit" size={18} aria-hidden /> : null}
                  {params.InputProps.endAdornment}
                </>
              ),
            }}
          />
        )}
        sx={{ flex: 1, minWidth: 0 }}
      />
      <Box
        sx={{
          width: { xs: "100%", sm: "auto" },
          flexShrink: 0,
          pt: { sm: helper ? 0 : 0.5 },
          alignSelf: { sm: "flex-start" },
        }}
      >
        {submitSlot}
      </Box>
    </Box>
  );
}
