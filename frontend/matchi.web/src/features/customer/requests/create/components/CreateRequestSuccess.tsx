import { Box, Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../../shared/i18n";
import { AppCard } from "../../../../../shared/ui/AppCard";

type CreateRequestSuccessProps = {
  requestId: number;
  onCreateAnother: () => void;
};

export function CreateRequestSuccess({ requestId, onCreateAnother }: CreateRequestSuccessProps) {
  return (
    <AppCard>
      <Stack spacing={2.5} alignItems="center" sx={{ py: { xs: 1, sm: 2 }, textAlign: "center" }}>
        <Box
          aria-hidden
          sx={{
            width: 64,
            height: 64,
            borderRadius: "50%",
            bgcolor: "success.main",
            color: "success.contrastText",
            display: "grid",
            placeItems: "center",
            fontSize: 28,
            fontWeight: 700,
          }}
        >
          ✓
        </Box>
        <Stack spacing={1}>
          <Typography variant="h4" component="h1">
            {t("request.create.successTitle")}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t("request.create.successBody")}
          </Typography>
        </Stack>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          spacing={1}
          sx={{ width: "100%", justifyContent: "center" }}
        >
          <Button
            component={RouterLink}
            to={`/customer/requests/${requestId}`}
            variant="contained"
            sx={{ minHeight: 48 }}
          >
            {t("request.create.viewRequest")}
          </Button>
          <Button
            component={RouterLink}
            to={`/customer/requests/${requestId}/matches`}
            variant="outlined"
            sx={{ minHeight: 48 }}
          >
            {t("request.create.viewMatches")}
          </Button>
          <Button type="button" variant="text" onClick={onCreateAnother} sx={{ minHeight: 48 }}>
            {t("request.create.another")}
          </Button>
        </Stack>
      </Stack>
    </AppCard>
  );
}
