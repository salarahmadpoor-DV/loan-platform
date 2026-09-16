import { Button, Stack, Typography } from "@mui/material";
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
  return (
    <Stack spacing={1.5}>
      <Stack role="radiogroup" aria-label={t("request.create.kind")} spacing={1.5}>
        {REQUEST_KINDS.map((kind) => {
          const selected = value === kind;
          return (
            <Button
              key={kind}
              role="radio"
              aria-checked={selected}
              variant={selected ? "contained" : "outlined"}
              color="primary"
              disabled={disabled}
              onClick={() => onChange(kind)}
              fullWidth
              sx={{
                justifyContent: "flex-start",
                textAlign: "start",
                py: 1.75,
                px: 2,
                minHeight: 56,
                whiteSpace: "normal",
                borderWidth: selected ? 2 : 1,
              }}
            >
              <Stack alignItems="flex-start" spacing={0.5}>
                <Typography component="span" fontWeight={700}>
                  {t(KIND_LABEL[kind])}
                </Typography>
                <Typography component="span" variant="body2" sx={{ opacity: 0.9 }}>
                  {t(KIND_HINT[kind])}
                </Typography>
              </Stack>
            </Button>
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
