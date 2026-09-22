import { Slider, Stack, Typography } from "@mui/material";
import { useState } from "react";
import { t } from "../../../../shared/i18n";
import { MapFrame } from "../../../../shared/map/MapFrame";
import {
  DEFAULT_SERVICE_RADIUS_KM,
  MAX_SERVICE_RADIUS_KM,
  MIN_SERVICE_RADIUS_KM,
  type ServiceAreaSelection,
} from "../model/serviceArea";

type ServiceAreaMapPickerProps = {
  value: ServiceAreaSelection | null;
  onChange: (next: ServiceAreaSelection) => void;
  error?: string;
};

export function ServiceAreaMapPicker({ value, onChange, error }: ServiceAreaMapPickerProps) {
  const [mapFailed, setMapFailed] = useState(false);
  const [draftRadiusKm, setDraftRadiusKm] = useState(
    value?.radiusKm ?? DEFAULT_SERVICE_RADIUS_KM,
  );
  const radiusKm = value?.radiusKm ?? draftRadiusKm;
  const sliderMin = Math.min(MIN_SERVICE_RADIUS_KM, Math.floor(radiusKm) || MIN_SERVICE_RADIUS_KM);
  const sliderMax = Math.max(MAX_SERVICE_RADIUS_KM, Math.ceil(radiusKm) || MAX_SERVICE_RADIUS_KM);
  const point = completeSelection(value);

  function selectCenter(lat: number, lng: number) {
    onChange({ lat, lng, radiusKm });
  }

  function changeRadius(_event: Event, next: number | number[]) {
    const nextRadius = Array.isArray(next) ? next[0] : next;
    setDraftRadiusKm(nextRadius);
    if (value && Number.isFinite(value.lat) && Number.isFinite(value.lng)) {
      onChange({ ...value, radiusKm: nextRadius });
    }
  }

  return (
    <Stack spacing={1.5} sx={{ minWidth: 0 }}>
      <Typography variant="subtitle1">{t("provider.serviceArea.title")}</Typography>
      <Typography variant="body2" color="text.secondary">
        {t("provider.serviceArea.description")}
      </Typography>
      <MapFrame
        value={point}
        onSelectCenter={selectCenter}
        circleRadiusMeters={point ? radiusKm * 1000 : undefined}
        error={Boolean(error)}
        mapFailed={mapFailed}
        onMapFailed={() => setMapFailed(true)}
        failedLabel={t("provider.serviceArea.mapFailed")}
        loadingLabel={t("provider.serviceArea.mapLoading")}
      />
      {error ? (
        <Typography variant="caption" color="error">
          {error}
        </Typography>
      ) : null}
      <Typography variant="body2">{t("provider.serviceArea.radius")}</Typography>
      <Slider
        min={sliderMin}
        max={sliderMax}
        step={1}
        value={radiusKm}
        onChange={changeRadius}
        valueLabelDisplay="auto"
        aria-label={t("provider.serviceArea.radius")}
      />
      <Typography variant="body2" color="text.secondary">
        {t("provider.serviceArea.radiusValue", { km: radiusKm })}
      </Typography>
    </Stack>
  );
}

function completeSelection(value: ServiceAreaSelection | null): ServiceAreaSelection | null {
  if (value == null || !Number.isFinite(value.lat) || !Number.isFinite(value.lng)) {
    return null;
  }
  return value;
}
