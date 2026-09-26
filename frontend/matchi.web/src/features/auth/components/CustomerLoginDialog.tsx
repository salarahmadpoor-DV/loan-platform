import { Dialog, DialogContent, IconButton, Stack, Typography, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useId } from "react";
import { useNavigate } from "react-router-dom";
import { matchiRadius, matchiShadows } from "../../../app/designTokens";
import { t } from "../../../shared/i18n";
import { customerDashboardPath } from "../../../shared/marketplace/publicPaths";
import { OtpLoginForm } from "./OtpLoginForm";

type CustomerLoginDialogProps = {
  open: boolean;
  nextPath?: string;
  onClose: () => void;
};

export function CustomerLoginDialog({ open, nextPath, onClose }: CustomerLoginDialogProps) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const navigate = useNavigate();
  const titleId = useId();

  function handleLoggedIn() {
    onClose();
    navigate(nextPath ?? customerDashboardPath, { replace: true });
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      fullScreen={isMobile}
      maxWidth="sm"
      scroll="body"
      aria-labelledby={titleId}
      slotProps={{
        backdrop: {
          sx: {
            bgcolor: "rgba(15, 23, 42, 0.45)",
            backdropFilter: "blur(8px)",
            WebkitBackdropFilter: "blur(8px)",
          },
        },
      }}
      PaperProps={{
        sx: {
          borderRadius: isMobile ? 0 : `${matchiRadius.lg}px`,
          boxShadow: matchiShadows.elevated,
          m: { xs: 0, sm: 2 },
          maxHeight: { xs: "100%", sm: "calc(100% - 64px)" },
        },
      }}
    >
      <Stack direction="row" justifyContent="flex-end" sx={{ px: 1, pt: 1 }}>
        <IconButton onClick={onClose} aria-label={t("auth.close")} size="small">
          <Typography component="span" aria-hidden fontWeight={700}>
            ×
          </Typography>
        </IconButton>
      </Stack>
      <DialogContent sx={{ pt: 0, px: { xs: 2.5, sm: 4 }, pb: { xs: 3, sm: 4 } }}>
        <OtpLoginForm
          titleId={titleId}
          headingComponent="h2"
          title={t("auth.customer.modalTitle")}
          description={t("auth.customer.modalDescription")}
          mobileSubmitLabel={t("auth.customer.sendCode")}
          onLoggedIn={handleLoggedIn}
          footer={
            <Typography variant="body2" color="text.secondary" sx={{ textAlign: "center", pt: 1 }}>
              {t("auth.customer.footer")}
            </Typography>
          }
        />
      </DialogContent>
    </Dialog>
  );
}
