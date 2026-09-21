import { Button, Container, Stack, TextField, Typography } from "@mui/material";
import { useState, type FormEvent } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../../../../shared/auth/AuthProvider";
import { useWorkspaceAccess } from "../../../../shared/auth/useWorkspaceAccess";
import { t } from "../../../../shared/i18n";
import { providerWorkspacePath, loginPathWithNext } from "../../../../shared/marketplace/publicPaths";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { FormSplitLayout } from "../../../../shared/ui/FormSplitLayout";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { useCreateMyProvider } from "../hooks/useCreateMyProvider";
import {
  defaultCreateProviderValues,
  toCreateProviderBody,
  validateCreateProviderForm,
  type CreateProviderFieldErrors,
  type CreateProviderFormValues,
} from "../model/createProviderForm";

function fieldError(errors: CreateProviderFieldErrors, key: keyof CreateProviderFieldErrors): string | undefined {
  const messageKey = errors[key];
  return messageKey ? t(messageKey) : undefined;
}

export function ProviderOnboardPage() {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const access = useWorkspaceAccess();
  const create = useCreateMyProvider();
  const [values, setValues] = useState<CreateProviderFormValues>(defaultCreateProviderValues);
  const [errors, setErrors] = useState<CreateProviderFieldErrors>({});

  if (!isAuthenticated) {
    return <Navigate to={loginPathWithNext("/provider/onboard")} replace />;
  }

  if (!access.isReady) {
    return <LoadingState />;
  }

  if (access.canAccess("provider")) {
    return <Navigate to={providerWorkspacePath} replace />;
  }

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    const nextErrors = validateCreateProviderForm(values);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }
    create.mutate(toCreateProviderBody(values), {
      onSuccess: () => {
        navigate(providerWorkspacePath, { replace: true });
      },
    });
  }

  return (
    <Container maxWidth="md" sx={{ py: { xs: 3, sm: 5 }, px: { xs: 2, sm: 3 } }}>
      <PageHeader
        title={t("provider.onboard.pageTitle")}
        description={t("provider.onboard.pageDescription")}
      />
      {create.isError ? <ErrorAlert error={create.error} /> : null}
      <FormSplitLayout
        main={
          <AppCard>
            <Stack component="form" spacing={2} onSubmit={onSubmit} noValidate>
              <TextField
                label={t("provider.onboard.name")}
                name="name"
                value={values.name}
                onChange={(event) => setValues((current) => ({ ...current, name: event.target.value }))}
                required
                fullWidth
                error={Boolean(errors.name)}
                helperText={fieldError(errors, "name")}
                inputProps={{ maxLength: 200 }}
              />
              <TextField
                label={t("provider.onboard.description")}
                name="description"
                value={values.description}
                onChange={(event) =>
                  setValues((current) => ({ ...current, description: event.target.value }))
                }
                fullWidth
                multiline
                minRows={3}
                error={Boolean(errors.description)}
                helperText={fieldError(errors, "description")}
                inputProps={{ maxLength: 2000 }}
              />
              <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                <TextField
                  label={t("provider.onboard.lat")}
                  name="lat"
                  value={values.lat}
                  onChange={(event) => setValues((current) => ({ ...current, lat: event.target.value }))}
                  fullWidth
                  error={Boolean(errors.lat)}
                  helperText={fieldError(errors, "lat")}
                  inputProps={{ inputMode: "decimal" }}
                />
                <TextField
                  label={t("provider.onboard.lng")}
                  name="lng"
                  value={values.lng}
                  onChange={(event) => setValues((current) => ({ ...current, lng: event.target.value }))}
                  fullWidth
                  error={Boolean(errors.lng)}
                  helperText={fieldError(errors, "lng")}
                  inputProps={{ inputMode: "decimal" }}
                />
              </Stack>
              <Button
                type="submit"
                variant="contained"
                size="large"
                disabled={create.isPending}
                sx={{ minHeight: 44, alignSelf: { xs: "stretch", sm: "flex-start" } }}
              >
                {create.isPending ? t("provider.onboard.submitting") : t("provider.onboard.submit")}
              </Button>
            </Stack>
          </AppCard>
        }
        summary={
          <AppCard>
            <Stack spacing={1.5}>
              <Typography variant="subtitle1">{t("provider.onboard.summaryTitle")}</Typography>
              <Typography variant="body2" color="text.secondary">
                {t("provider.onboard.hint")}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {t("provider.onboard.account", { mobile: user?.mobile ?? t("common.notSpecified") })}
              </Typography>
            </Stack>
          </AppCard>
        }
      />
    </Container>
  );
}
