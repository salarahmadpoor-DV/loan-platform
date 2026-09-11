import { ToggleButton, ToggleButtonGroup } from "@mui/material";
import { t, type MessageKey } from "../../../../../shared/i18n";
import { REQUEST_KINDS, type RequestKind } from "../../../../../shared/types/marketplace";

const KIND_LABEL: Record<RequestKind, MessageKey> = {
  Service: "request.kind.Service",
  Product: "request.kind.Product",
  Hybrid: "request.kind.Hybrid",
};

type RequestKindSelectorProps = {
  value: RequestKind;
  onChange: (value: RequestKind) => void;
  disabled?: boolean;
};

export function RequestKindSelector({ value, onChange, disabled }: RequestKindSelectorProps) {
  return (
    <ToggleButtonGroup
      exclusive
      fullWidth
      color="primary"
      value={value}
      disabled={disabled}
      onChange={(_event, next: RequestKind | null) => {
        if (next) {
          onChange(next);
        }
      }}
      aria-label={t("request.create.kind")}
    >
      {REQUEST_KINDS.map((kind) => (
        <ToggleButton key={kind} value={kind} sx={{ py: 1.25 }}>
          {t(KIND_LABEL[kind])}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
}
