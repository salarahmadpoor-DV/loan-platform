import { Alert, Button, Stack } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import type { ProviderProfile } from "../api/providerProfileTypes";
import { useMyProviderServiceAreas } from "../hooks/useMyProviderServiceAreas";
import { useSaveProviderServiceArea } from "../hooks/useSaveProviderServiceArea";
import {
  DEFAULT_SERVICE_RADIUS_KM,
  RADIUS_AREA_TYPE,
  isValidServiceArea,
  type ServiceAreaSelection,
} from "../model/serviceArea";
import { ServiceAreaMapPicker } from "./ServiceAreaMapPicker";

type ProviderServiceAreaSectionProps = {
  profile: ProviderProfile;
};

export function ProviderServiceAreaSection({ profile }: ProviderServiceAreaSectionProps) {
  const areas = useMyProviderServiceAreas();
  const save = useSaveProviderServiceArea();
  const [selection, setSelection] = useState<ServiceAreaSelection | null>(null);
  const [areaId, setAreaId] = useState<number | undefined>();
  const [initialized, setInitialized] = useState(false);
  const [selectionError, setSelectionError] = useState<string | undefined>();
  const [saved, setSaved] = useState(false);

  const existingRadiusArea = useMemo(() => {
    return areas.data?.find(
      (area) =>
        area.areaType === RADIUS_AREA_TYPE &&
        area.isActive &&
        area.lat != null &&
        area.lng != null &&
        area.radius != null &&
        area.radius > 0,
    );
  }, [areas.data]);

  useEffect(() => {
    if (!areas.data || initialized) {
      return;
    }
    if (existingRadiusArea && existingRadiusArea.lat != null && existingRadiusArea.lng != null) {
      setSelection({
        lat: existingRadiusArea.lat,
        lng: existingRadiusArea.lng,
        radiusKm: Number(existingRadiusArea.radius),
      });
      setAreaId(existingRadiusArea.id);
    } else if (profile.lat != null && profile.lng != null) {
      setSelection({
        lat: profile.lat,
        lng: profile.lng,
        radiusKm: DEFAULT_SERVICE_RADIUS_KM,
      });
      setAreaId(undefined);
    } else {
      setSelection(null);
      setAreaId(undefined);
    }
    setInitialized(true);
  }, [areas.data, existingRadiusArea, initialized, profile.lat, profile.lng]);

  function onConfirm() {
    setSaved(false);
    if (!isValidServiceArea(selection)) {
      setSelectionError(t("provider.serviceArea.required"));
      return;
    }
    setSelectionError(undefined);
    save.mutate(
      {
        selection,
        existingAreaId: areaId,
        profile: {
          name: profile.name,
          description: profile.description,
          mobile: profile.mobile,
        },
      },
      {
        onSuccess: (savedAreaId) => {
          setSaved(true);
          if (savedAreaId) {
            setAreaId(savedAreaId);
          }
        },
      },
    );
  }

  return (
    <AppCard>
      <Stack spacing={2}>
        {areas.isPending ? <LoadingState label={t("provider.serviceArea.loading")} /> : null}
        {areas.isError ? (
          <Stack spacing={1}>
            <ErrorAlert error={areas.error} />
            <Button
              variant="outlined"
              onClick={() => {
                void areas.refetch();
              }}
              disabled={areas.isFetching}
              sx={{ minHeight: 44, alignSelf: "flex-start" }}
            >
              {t("provider.serviceArea.retry")}
            </Button>
          </Stack>
        ) : null}
        {save.isError ? <ErrorAlert error={save.error} /> : null}
        {saved ? <Alert severity="success">{t("provider.serviceArea.saved")}</Alert> : null}
        {!areas.isPending && !areas.isError ? (
          <>
            <ServiceAreaMapPicker
              value={selection}
              onChange={(next) => {
                setSaved(false);
                setSelectionError(undefined);
                setSelection(next);
              }}
              error={selectionError}
            />
            <Button
              variant="contained"
              onClick={onConfirm}
              disabled={save.isPending}
              sx={{ minHeight: 44, alignSelf: { xs: "stretch", sm: "flex-start" } }}
            >
              {save.isPending ? t("provider.serviceArea.saving") : t("provider.serviceArea.confirm")}
            </Button>
          </>
        ) : null}
      </Stack>
    </AppCard>
  );
}
