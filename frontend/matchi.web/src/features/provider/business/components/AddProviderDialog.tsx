import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState, type FormEvent } from "react";
import { t } from "../../../../shared/i18n";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { useInviteBusinessProvider } from "../hooks/useBusinessTeamMutations";

const MOBILE_PATTERN = /^\+?\d{8,15}$/;

type AddProviderDialogProps = {
  open: boolean;
  businessId: number;
  onClose: () => void;
};

export function AddProviderDialog({ open, businessId, onClose }: AddProviderDialogProps) {
  const invite = useInviteBusinessProvider(businessId);
  const [mobile, setMobile] = useState("");
  const [localError, setLocalError] = useState<string | null>(null);

  function handleClose() {
    setMobile("");
    setLocalError(null);
    invite.reset();
    onClose();
  }

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    const trimmed = mobile.trim();
    if (!MOBILE_PATTERN.test(trimmed)) {
      setLocalError(t("provider.business.invalidMobile"));
      return;
    }
    setLocalError(null);
    invite.mutate(
      { mobile: trimmed, role: "Member" },
      {
        onSuccess: () => {
          handleClose();
        },
      },
    );
  }

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm" aria-labelledby="add-provider-title">
      <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ pr: 1 }}>
        <DialogTitle id="add-provider-title" sx={{ flex: 1 }}>
          {t("provider.business.inviteTitle")}
        </DialogTitle>
        <IconButton onClick={handleClose} aria-label={t("provider.business.close")} size="small">
          <Typography component="span" aria-hidden fontWeight={700}>
            ×
          </Typography>
        </IconButton>
      </Stack>
      <DialogContent>
        <Stack component="form" id="add-provider-form" spacing={2} onSubmit={onSubmit} noValidate>
          <Typography variant="body2" color="text.secondary">
            {t("provider.business.inviteHint")}
          </Typography>
          {localError ? (
            <Typography variant="body2" color="error">
              {localError}
            </Typography>
          ) : null}
          {invite.isError ? <ErrorAlert error={invite.error} /> : null}
          <TextField
            label={t("provider.business.providerMobile")}
            name="mobile"
            type="tel"
            autoComplete="tel"
            value={mobile}
            onChange={(event) => setMobile(event.target.value)}
            required
            fullWidth
            inputProps={{ inputMode: "tel" }}
          />
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={handleClose}>{t("provider.business.cancel")}</Button>
        <Button type="submit" form="add-provider-form" variant="contained" disabled={invite.isPending}>
          {invite.isPending ? t("provider.business.sending") : t("provider.business.sendInvite")}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
