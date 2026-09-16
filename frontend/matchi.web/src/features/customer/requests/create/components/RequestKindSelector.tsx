import { Box, Stack, Typography } from "@mui/material";
import { alpha, useTheme } from "@mui/material/styles";
import { t, type MessageKey } from "../../../../../shared/i18n";
import { REQUEST_KINDS, type RequestKind } from "../../../../../shared/types/marketplace";

const KIND_LABEL: Record<RequestKind, MessageKey> = {
  Service: "request.kind.Service",
  Product: "request.kind.Product",
  Hybrid: "request.kind.Hybrid",
};

const KIND_HINT: Record<RequestKind, MessageKey> = {
  Service: "request.create.kindHint.Service",
  Product: "request.create.kindHint.Product",
  Hybrid: "request.create.kindHint.Hybrid",
};

type RequestKindSelectorProps = {
  value: RequestKind;
  onChange: (value: RequestKind) => void;
  disabled?: boolean;
  error?: string;
};

export function RequestKindSelector({
  value,
  onChange,
  disabled,
  error,
}: RequestKindSelectorProps) {
  const theme = useTheme();

  return (
    <Stack spacing={1.5}>
      <Stack role="radiogroup" aria-label={t("request.create.kind")} spacing={1.5}>
        {REQUEST_KINDS.map((kind) => {
          const selected = value === kind;
          return (
            <Box
              key={kind}
              component="button"
              type="button"
              role="radio"
              aria-checked={selected}
              disabled={disabled}
              onClick={() => onChange(kind)}
              sx={{
                display: "block",
                width: "100%",
                textAlign: "start",
                cursor: disabled ? "default" : "pointer",
                py: 1.75,
                px: 2,
                minHeight: 56,
                borderRadius: 2,
                border: "2px solid",
                borderColor: selected ? "primary.main" : "divider",
                bgcolor: selected ? alpha(theme.palette.primary.main, 0.06) : "background.paper",
                color: "text.primary",
                font: "inherit",
                "&:hover": disabled
                  ? undefined
                  : {
                      borderColor: "primary.light",
                      bgcolor: alpha(theme.palette.primary.main, 0.04),
                    },
                "&:focus-visible": {
                  outline: "2px solid",
                  outlineColor: "primary.main",
                  outlineOffset: 2,
                },
                "&:disabled": {
                  opacity: 0.6,
                },
              }}
            >
              <Stack alignItems="flex-start" spacing={0.5}>
                <Typography component="span" fontWeight={700}>
                  {t(KIND_LABEL[kind])}
                </Typography>
                <Typography component="span" variant="body2" color="text.secondary">
                  {t(KIND_HINT[kind])}
                </Typography>
              </Stack>
            </Box>
          );
        })}
      </Stack>
      {error ? (
        <Typography color="error" variant="caption" role="alert">
          {error}
        </Typography>
      ) : null}
    </Stack>
  );
}
