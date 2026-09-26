import { Button, Stack, TextField } from "@mui/material";
import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { useCreateOwnedBusiness } from "../hooks/useCreateOwnedBusiness";

type FormState = {
  name: string;
  description: string;
  mobile: string;
  address: string;
  province: string;
  city: string;
  district: string;
};

const emptyForm: FormState = {
  name: "",
  description: "",
  mobile: "",
  address: "",
  province: "",
  city: "",
  district: "",
};

function emptyToNull(value: string): string | null {
  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

export function ProviderBusinessCreatePage() {
  const navigate = useNavigate();
  const create = useCreateOwnedBusiness();
  const [values, setValues] = useState<FormState>(emptyForm);
  const [nameError, setNameError] = useState<string | undefined>();

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    const name = values.name.trim();
    if (!name) {
      setNameError(t("provider.business.nameRequired"));
      return;
    }
    setNameError(undefined);
    create.mutate(
      {
        name,
        description: emptyToNull(values.description),
        mobile: emptyToNull(values.mobile),
        address: emptyToNull(values.address),
        province: emptyToNull(values.province),
        city: emptyToNull(values.city),
        district: emptyToNull(values.district),
      },
      {
        onSuccess: () => {
          navigate("/provider/business", { replace: true });
        },
      },
    );
  }

  return (
    <>
      <PageHeader
        title={t("provider.business.createTitle")}
        description={t("provider.business.createDescription")}
      />
      {create.isError ? <ErrorAlert error={create.error} /> : null}
      <AppCard>
        <Stack component="form" spacing={2} onSubmit={onSubmit} noValidate>
          <TextField
            label={t("provider.business.name")}
            name="name"
            value={values.name}
            onChange={(event) => setValues((current) => ({ ...current, name: event.target.value }))}
            required
            fullWidth
            error={Boolean(nameError)}
            helperText={nameError}
          />
          <TextField
            label={t("provider.business.description")}
            name="description"
            value={values.description}
            onChange={(event) => setValues((current) => ({ ...current, description: event.target.value }))}
            multiline
            minRows={3}
            fullWidth
          />
          <TextField
            label={t("provider.business.mobile")}
            name="mobile"
            type="tel"
            value={values.mobile}
            onChange={(event) => setValues((current) => ({ ...current, mobile: event.target.value }))}
            fullWidth
          />
          <TextField
            label={t("provider.business.address")}
            name="address"
            value={values.address}
            onChange={(event) => setValues((current) => ({ ...current, address: event.target.value }))}
            fullWidth
          />
          <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
            <TextField
              label={t("provider.business.province")}
              name="province"
              value={values.province}
              onChange={(event) => setValues((current) => ({ ...current, province: event.target.value }))}
              fullWidth
            />
            <TextField
              label={t("provider.business.city")}
              name="city"
              value={values.city}
              onChange={(event) => setValues((current) => ({ ...current, city: event.target.value }))}
              fullWidth
            />
          </Stack>
          <TextField
            label={t("provider.business.district")}
            name="district"
            value={values.district}
            onChange={(event) => setValues((current) => ({ ...current, district: event.target.value }))}
            fullWidth
          />
          <Button type="submit" variant="contained" disabled={create.isPending} sx={{ alignSelf: "flex-start" }}>
            {create.isPending ? t("provider.business.creating") : t("provider.business.create")}
          </Button>
        </Stack>
      </AppCard>
    </>
  );
}
