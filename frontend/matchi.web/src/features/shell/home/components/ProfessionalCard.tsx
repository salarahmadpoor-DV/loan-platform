import { Avatar, Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import type { MockProfessional } from "../../../../shared/mocks/homeMocks";
import { AppCard } from "../../../../shared/ui/AppCard";
import { RatingStars } from "./RatingStars";
import { VerificationBadge } from "./VerificationBadge";

type ProfessionalCardProps = {
  professional: MockProfessional;
  onStartRequest: () => void;
};

export function ProfessionalCard({ professional, onStartRequest }: ProfessionalCardProps) {
  const initial = professional.name.trim().charAt(0) || "?";

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack direction="row" spacing={1.5} alignItems="center">
          <Avatar
            sx={{
              width: 56,
              height: 56,
              bgcolor: "primary.main",
              color: "primary.contrastText",
              typography: "h6",
            }}
            alt=""
          >
            {initial}
          </Avatar>
          <Stack spacing={0.25} sx={{ minWidth: 0 }}>
            <Typography variant="subtitle1" component="h3" noWrap>
              {professional.name}
            </Typography>
            <Typography variant="body2" color="text.secondary" noWrap>
              {t(professional.professionKey)}
            </Typography>
          </Stack>
        </Stack>
        <VerificationBadge />
        <RatingStars
          value={professional.rating}
          reviewCount={professional.reviewCount}
          reviewLabel={t("public.pros.reviews", { count: professional.reviewCount })}
        />
        <Typography variant="caption" color="text.secondary">
          {t(professional.locationKey)}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t(professional.bioKey)}
        </Typography>
        {professional.startingPrice ? (
          <Typography variant="subtitle2">
            {t("public.pros.fromPrice", { price: professional.startingPrice })}
          </Typography>
        ) : null}
        <Button variant="contained" onClick={onStartRequest} sx={{ alignSelf: { xs: "stretch", sm: "flex-start" } }}>
          {t("public.pros.startRequest")}
        </Button>
      </Stack>
    </AppCard>
  );
}
