import { Button, MenuItem, Stack, TextField } from "@mui/material";
import type { ReactNode } from "react";
import { Link as RouterLink } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";
import { useSelectedOwnedBusiness } from "../hooks/useSelectedOwnedBusiness";

type OwnedBusinessPageProps = {
  title: string;
  description?: string;
  action?: ReactNode;
  children: (business: MyBusinessProfile) => ReactNode;
};

export function OwnedBusinessPage({ title, description, action, children }: OwnedBusinessPageProps) {
  const owned = useSelectedOwnedBusiness();

  if (owned.isPending) {
    return (
      <>
        <PageHeader title={title} description={description} />
        <LoadingState />
      </>
    );
  }

  if (owned.isError) {
    return (
      <>
        <PageHeader title={title} description={description} />
        <Stack spacing={2}>
          <ErrorAlert error={owned.error} />
          <Button variant="outlined" onClick={() => void owned.refetch()} sx={{ alignSelf: "flex-start" }}>
            {t("error.retry")}
          </Button>
        </Stack>
      </>
    );
  }

  if (!owned.business) {
    return (
      <>
        <PageHeader title={title} description={description} />
        <EmptyState
          title={t("provider.business.emptyTitle")}
          body={t("provider.business.emptyBody")}
          action={
            <Button
              component={RouterLink}
              to="/provider/business/create"
              variant="contained"
              sx={{ minHeight: 44 }}
            >
              {t("provider.business.create")}
            </Button>
          }
        />
      </>
    );
  }

  return (
    <>
      <PageHeader title={title} description={description} action={action} />
      {owned.businesses.length > 1 ? (
        <TextField
          select
          size="small"
          label={t("provider.business.selectBusiness")}
          value={owned.business.id}
          onChange={(event) => owned.setSelectedId(Number(event.target.value))}
          sx={{ mb: 2, minWidth: { xs: "100%", sm: 280 } }}
        >
          {owned.businesses.map((item) => (
            <MenuItem key={item.id} value={item.id}>
              {item.name}
            </MenuItem>
          ))}
        </TextField>
      ) : null}
      {children(owned.business)}
    </>
  );
}
