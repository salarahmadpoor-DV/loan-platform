import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { FormSplitLayout } from "../../../../shared/ui/FormSplitLayout";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
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
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
          >
            {t("provider.profile.retry")}
          </Button>
        </Stack>
      ) : null}
      {data ? (
        <FormSplitLayout
          wide
          main={
            <AppCard>
              <Stack spacing={1.5}>
                <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                  <StatusChip label={data.status} tone="info" />
                </Stack>
                <Typography variant="caption" color="text.secondary">
                  {t("provider.profile.name")}
                </Typography>
                <Typography variant="h6">{data.name}</Typography>
                <Typography variant="body2">
                  {t("provider.profile.mobile")}: {data.mobile}
                </Typography>
                <Typography variant="body1" sx={{ whiteSpace: "pre-wrap" }}>
                  {data.description?.trim() ? data.description : t("provider.profile.noDescription")}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {data.lat != null && data.lng != null
                    ? t("provider.profile.coordinatesValue", { lat: data.lat, lng: data.lng })
                    : t("provider.profile.noCoordinates")}
                </Typography>
              </Stack>
            </AppCard>
          }
          summary={
            <AppCard>
              <Stack spacing={1.5}>
                <Typography variant="subtitle1">{t("provider.profile.recorded")}</Typography>
                <Typography variant="body2">
                  {t("provider.profile.rating", { rating: data.rating })}
                </Typography>
                <Typography variant="body2">
                  {t("provider.profile.reviewCount", { count: data.reviewCount })}
                </Typography>
                <Typography variant="body2">
                  {t("provider.profile.completedJobs", { count: data.completedJobCount })}
                </Typography>
              </Stack>
            </AppCard>
          }
        />
      ) : null}
    </>
  );
}
