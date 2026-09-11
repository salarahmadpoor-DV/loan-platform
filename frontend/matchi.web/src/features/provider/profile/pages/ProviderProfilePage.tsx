import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { useMyProviderProfile } from "../hooks/useMyProviderProfile";

export function ProviderProfilePage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderProfile();

  return (
    <>
      <PageHeader
        title={t("provider.profile.title")}
        description={t("provider.profile.description")}
      />
      {isPending ? <LoadingState label={t("provider.profile.loading")} /> : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ alignSelf: "flex-start" }}
          >
            {t("provider.profile.retry")}
          </Button>
        </Stack>
      ) : null}
      {data ? (
        <AppCard>
          <Stack spacing={1}>
            <Typography variant="body2">
              {t("provider.profile.status")}: {data.status}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t("provider.profile.name")}
            </Typography>
            <Typography variant="h6">{data.name}</Typography>
            <Typography variant="body2">
              {t("provider.profile.mobile")}: {data.mobile}
            </Typography>
            <Typography variant="body2">
              {t("provider.profile.rating", { rating: data.rating })}
            </Typography>
            <Typography variant="body2">
              {t("provider.profile.reviewCount", { count: data.reviewCount })}
            </Typography>
            <Typography variant="body2">
              {t("provider.profile.completedJobs", { count: data.completedJobCount })}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
              {data.description?.trim() ? data.description : t("provider.profile.noDescription")}
            </Typography>
          </Stack>
        </AppCard>
      ) : null}
    </>
  );
}
