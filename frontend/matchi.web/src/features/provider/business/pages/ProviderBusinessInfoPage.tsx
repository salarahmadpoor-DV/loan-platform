import { Alert, Button, Stack, TextField } from "@mui/material";
import { useEffect, useState, type FormEvent } from "react";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import type { MyBusinessProfile } from "../../profile/api/myBusinessTypes";
import { OwnedBusinessPage } from "../components/OwnedBusinessPage";
import { useUpdateOwnedBusiness } from "../hooks/useUpdateOwnedBusiness";

type FormState = {
  name: string;
  description: string;
  mobile: string;
  address: string;
  province: string;
  city: string;
  district: string;
};

function emptyToNull(value: string): string | null {
  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

function fromBusiness(business: MyBusinessProfile): FormState {
  return {
    name: business.name,
    description: business.description ?? "",
    mobile: business.mobile ?? "",
    address: business.address ?? "",
    province: business.province ?? "",
    city: business.city ?? "",
    district: business.district ?? "",
  };
}

function InfoForm({ business }: { business: MyBusinessProfile }) {
  const update = useUpdateOwnedBusiness();
  const [values, setValues] = useState<FormState>(() => fromBusiness(business));
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    setValues(fromBusiness(business));
    setSaved(false);
  }, [business]);

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSaved(false);
    update.mutate(
      {
        businessId: business.id,
        name: values.name.trim(),
        description: emptyToNull(values.description),
        mobile: emptyToNull(values.mobile),
        address: emptyToNull(values.address),
        province: emptyToNull(values.province),
        city: emptyToNull(values.city),
        district: emptyToNull(values.district),
        lat: business.lat,
        lng: business.lng,
        logoMediaId: business.logoMediaId,
      },
      { onSuccess: () => setSaved(true) },
    );
  }

  return (
    <AppCard>
      <Stack component="form" spacing={2} onSubmit={onSubmit} noValidate>
        {update.isError ? <ErrorAlert error={update.error} /> : null}
        {saved ? <Alert severity="success">{t("provider.business.saved")}</Alert> : null}
        <TextField
          label={t("provider.business.name")}
          value={values.name}
          onChange={(event) => setValues((current) => ({ ...current, name: event.target.value }))}
          required
          fullWidth
        />
        <TextField
          label={t("provider.business.description")}
          value={values.description}
          onChange={(event) => setValues((current) => ({ ...current, description: event.target.value }))}
          multiline
          minRows={3}
          fullWidth
        />
        <TextField
          label={t("provider.business.mobile")}
          value={values.mobile}
          onChange={(event) => setValues((current) => ({ ...current, mobile: event.target.value }))}
          fullWidth
        />
        <TextField
          label={t("provider.business.address")}
          value={values.address}
          onChange={(event) => setValues((current) => ({ ...current, address: event.target.value }))}
          fullWidth
        />
        <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
          <TextField
            label={t("provider.business.province")}
            value={values.province}
            onChange={(event) => setValues((current) => ({ ...current, province: event.target.value }))}
            fullWidth
          />
          <TextField
            label={t("provider.business.city")}
            value={values.city}
            onChange={(event) => setValues((current) => ({ ...current, city: event.target.value }))}
            fullWidth
          />
        </Stack>
        <TextField
          label={t("provider.business.district")}
          value={values.district}
          onChange={(event) => setValues((current) => ({ ...current, district: event.target.value }))}
          fullWidth
        />
        <Button type="submit" variant="contained" disabled={update.isPending} sx={{ alignSelf: "flex-start" }}>
          {update.isPending ? t("provider.business.saving") : t("provider.business.save")}
        </Button>
      </Stack>
    </AppCard>
  );
}

export function ProviderBusinessInfoPage() {
  return (
    <OwnedBusinessPage
      title={t("provider.business.infoTitle")}
      description={t("provider.business.infoDescription")}
    >
      {(business) => <InfoForm business={business} />}
    </OwnedBusinessPage>
  );
}
