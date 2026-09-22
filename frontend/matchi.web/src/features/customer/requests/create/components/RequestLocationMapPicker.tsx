import { Alert, Stack, TextField, Typography } from "@mui/material";
import { useRef, useState } from "react";
import { LocationSelector } from "../../../../location/LocationSelector";
import { resolveLocation } from "../../../../location/locationApi";
import type { LocationSelection } from "../../../../location/types";
import { t } from "../../../../../shared/i18n";
import { MapFrame } from "../../../../../shared/map/MapFrame";
import { isValidMapPoint, type MapPoint } from "../../../../../shared/map/mapConstants";

type ResolveStatus = "idle" | "loading" | "success" | "failure";

type RequestLocationMapPickerProps = {
  value: MapPoint | null;
  onChange: (next: MapPoint) => void;
  onResolved?: (location: LocationSelection) => void;
  error?: string;
};

export function RequestLocationMapPicker({
  value,
  onChange,
  onResolved,
  error,
}: RequestLocationMapPickerProps) {
  const [mapFailed, setMapFailed] = useState(false);
  const [resolveStatus, setResolveStatus] = useState<ResolveStatus>("idle");
  const requestId = useRef(0);
  const selected = isValidMapPoint(value);

  async function handleSelect(lat: number, lng: number) {
    onChange({ lat, lng });
    const current = requestId.current + 1;
    requestId.current = current;
    setResolveStatus("loading");
    try {
      const response = await resolveLocation(lat, lng);
      if (requestId.current !== current) {
        return;
      }
      if (response.success && response.data) {
        onResolved?.({
          provinceId: response.data.provinceId,
          cityId: response.data.cityId,
          districtId: response.data.districtId,
          provinceName: response.data.provinceName,
          cityName: response.data.cityName,
          districtName: response.data.districtName,
        });
        setResolveStatus("success");
        return;
      }
      setResolveStatus("failure");
    } catch {
      if (requestId.current !== current) {
        return;
      }
      setResolveStatus("failure");
    }
  }

  const mapAlertSeverity =
    resolveStatus === "loading"
      ? "info"
      : resolveStatus === "success"
        ? "success"
        : resolveStatus === "failure"
          ? "warning"
          : selected
            ? "success"
            : "info";

  const mapAlertMessage =
    resolveStatus === "loading"
      ? t("location.resolve.loading")
      : resolveStatus === "success"
        ? t("location.resolve.success")
        : resolveStatus === "failure"
          ? t("location.resolve.failure")
          : selected
            ? t("request.create.mapSelected")
            : t("request.create.mapNotSelected");

  return (
    <Stack spacing={1.5} sx={{ minWidth: 0 }}>
      <Typography variant="subtitle1">{t("request.create.mapTitle")}</Typography>
      <Typography variant="body2" color="text.secondary">
        {t("request.create.mapDescription")}
      </Typography>
      <MapFrame
        value={selected ? value : null}
        onSelectCenter={(lat, lng) => {
          void handleSelect(lat, lng);
        }}
        error={Boolean(error)}
        mapFailed={mapFailed}
        onMapFailed={() => setMapFailed(true)}
        failedLabel={t("request.create.mapFailed")}
        loadingLabel={t("request.create.mapLoading")}
      />
      {error ? (
        <Typography variant="caption" color="error">
          {error}
        </Typography>
      ) : (
        <Alert severity={mapAlertSeverity}>{mapAlertMessage}</Alert>
      )}
    </Stack>
  );
}

type RequestPlaceFieldsProps = {
  location: LocationSelection;
  address: string;
  errors: Partial<Record<"provinceId" | "cityId" | "districtId" | "address", string>>;
  disabled?: boolean;
  onLocationChange: (location: LocationSelection) => void;
  onAddressChange: (address: string) => void;
};

export function RequestPlaceFields({
  location,
  address,
  errors,
  disabled,
  onLocationChange,
  onAddressChange,
}: RequestPlaceFieldsProps) {
  return (
    <Stack spacing={2}>
      <Typography variant="body2" color="text.secondary">
        {t("request.create.placeHint")}
      </Typography>
      <LocationSelector
        value={location}
        onChange={onLocationChange}
        errors={{
          provinceId: errors.provinceId,
          cityId: errors.cityId,
          districtId: errors.districtId,
        }}
        disabled={disabled}
      />
      <TextField
        label={t("request.create.address")}
        value={address}
        onChange={(event) => onAddressChange(event.target.value)}
        error={Boolean(errors.address)}
        helperText={errors.address}
        disabled={disabled}
        fullWidth
        multiline
        minRows={2}
        inputProps={{ maxLength: 1000 }}
      />
    </Stack>
  );
}
