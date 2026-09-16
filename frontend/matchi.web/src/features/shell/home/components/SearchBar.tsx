import { Box, TextField } from "@mui/material";
import type { FormEvent, ReactNode } from "react";

type SearchBarProps = {
  id: string;
  label: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  suggestions?: string[];
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
  error,
  submitSlot,
}: SearchBarProps) {
  const listId = `${id}-suggestions`;

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    onSubmit();
  }

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{
        display: "flex",
        flexDirection: { xs: "column", sm: "row" },
        gap: 1.5,
        alignItems: { sm: "flex-start" },
      }}
    >
      <TextField
        id={id}
        label={label}
        placeholder={placeholder}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        error={Boolean(error)}
        helperText={error}
        inputProps={{
          list: suggestions.length > 0 ? listId : undefined,
          autoComplete: "off",
        }}
        sx={{ flex: 1 }}
      />
      {suggestions.length > 0 ? (
        <datalist id={listId}>
          {suggestions.map((item) => (
            <option key={item} value={item} />
          ))}
        </datalist>
      ) : null}
      <Box sx={{ width: { xs: "100%", sm: "auto" }, flexShrink: 0, pt: { sm: 0.5 } }}>{submitSlot}</Box>
    </Box>
  );
}
