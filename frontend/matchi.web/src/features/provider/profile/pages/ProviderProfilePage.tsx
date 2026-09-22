import { Button, Stack, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { FormSplitLayout } from "../../../../shared/ui/FormSplitLayout";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { ProviderServiceAreaSection } from "../components/ProviderServiceAreaSection";
import { useMyBusinesses } from "../hooks/useMyBusinesses";
import { useMyProviderMemberships } from "../hooks/useMyProviderMemberships";
import { useMyProviderProfile } from "../hooks/useMyProviderProfile";

export function ProviderProfilePage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderProfile();
  const owned = useMyBusinesses();
  const memberships = useMyProviderMemberships();

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

      {data ? (
        <Stack spacing={1.5} sx={{ mt: 3 }}>
          <ProviderServiceAreaSection profile={data} />
        </Stack>
      ) : null}

      <Stack spacing={1.5} sx={{ mt: 3 }}>
        <Typography variant="h6">{t("provider.profile.ownedTitle")}</Typography>
        <Typography variant="body2" color="text.secondary">
          {t("provider.profile.ownedBody")}
        </Typography>
        {owned.isPending ? <LoadingState /> : null}
        {owned.isError ? (
          <Stack spacing={1}>
            <ErrorAlert error={owned.error} />
            <Button
              variant="outlined"
              onClick={() => {
                void owned.refetch();
              }}
              disabled={owned.isFetching}
              sx={{ minHeight: 44, alignSelf: "flex-start" }}
            >
              {t("provider.profile.businessesRetry")}
            </Button>
          </Stack>
        ) : null}
        {owned.data && owned.data.length === 0 ? (
          <EmptyState
            title={t("provider.business.emptyTitle")}
            body={t("provider.business.emptyBody")}
            action={
              <Button
                component={RouterLink}
                to="/provider/business/create"
                variant="contained"
                sx={{ minHeight: 48 }}
              >
                {t("provider.business.create")}
              </Button>
            }
          />
        ) : null}
        {owned.data && owned.data.length > 0 ? (
          <Stack spacing={1.5}>
            {owned.data.map((business) => (
              <AppCard key={business.id}>
                <Stack spacing={0.75}>
                  <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                    <Typography variant="subtitle1">{business.name}</Typography>
                    <StatusChip label={business.status} tone="info" />
                  </Stack>
                  {business.city || business.province ? (
                    <Typography variant="body2" color="text.secondary">
                      {[business.city, business.province].filter(Boolean).join(" · ")}
                    </Typography>
                  ) : null}
                </Stack>
              </AppCard>
            ))}
          </Stack>
        ) : null}
        <Button
          component={RouterLink}
          to="/provider/offerings"
          variant="contained"
          sx={{ alignSelf: "flex-start", minHeight: 48 }}
        >
          {t("nav.offerings")}
        </Button>
        <Button
          component={RouterLink}
          to="/provider/business"
          variant="outlined"
          sx={{ alignSelf: "flex-start", minHeight: 48 }}
        >
          {t("provider.business.navSection")}
        </Button>
      </Stack>

      <Stack spacing={1.5} sx={{ mt: 3 }}>
        <Typography variant="h6">{t("provider.profile.membershipsTitle")}</Typography>
        <Typography variant="body2" color="text.secondary">
          {t("provider.profile.membershipsBody")}
        </Typography>
        {memberships.isPending ? <LoadingState /> : null}
        {memberships.isError ? (
          <Stack spacing={1}>
            <ErrorAlert error={memberships.error} />
            <Button
              variant="outlined"
              onClick={() => {
                void memberships.refetch();
              }}
              disabled={memberships.isFetching}
              sx={{ minHeight: 44, alignSelf: "flex-start" }}
            >
              {t("provider.profile.businessesRetry")}
            </Button>
          </Stack>
        ) : null}
        {memberships.data && memberships.data.length === 0 ? (
          <EmptyState title={t("provider.profile.membershipsEmpty")} />
        ) : null}
        {memberships.data && memberships.data.length > 0 ? (
          <Stack spacing={1.5}>
            {memberships.data.map((item) => (
              <AppCard key={`${item.businessId}-${item.joinedAt}`}>
                <Stack spacing={0.5}>
                  <Typography variant="subtitle1">
                    {item.businessName?.trim() || t("provider.profile.membershipBusiness", { id: item.businessId })}
                  </Typography>
                  <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
                    <StatusChip label={item.status} tone="neutral" />
                    <Typography variant="body2" color="text.secondary">
                      {t("provider.profile.membershipRole", { role: item.role })}
                    </Typography>
                  </Stack>
                </Stack>
              </AppCard>
            ))}
          </Stack>
        ) : null}
      </Stack>
    </>
  );
}
