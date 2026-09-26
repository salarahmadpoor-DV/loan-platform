import { Stack, Typography } from "@mui/material";
import { useState } from "react";
import { t } from "../../../../shared/i18n";
import { MapFrame } from "../../../../shared/map/MapFrame";
import { isValidMapPoint } from "../../../../shared/map/mapConstants";
import type { RequestLocation } from "../api/requestTypes";

type RequestLocationMapViewProps = {
  location: RequestLocation;
};

export function RequestLocationMapView({ location }: RequestLocationMapViewProps) {
  const [mapFailed, setMapFailed] = useState(false);
  const point =
    location.lat != null && location.lng != null
      ? { lat: location.lat, lng: location.lng }
      : null;

  if (!isValidMapPoint(point)) {
    return null;
  }

  return (
    <Stack spacing={1} sx={{ mt: 1.5, minWidth: 0 }}>
      <Typography variant="body2" color="text.secondary">
        {t("request.detail.mapSelected")}
      </Typography>
      <MapFrame
        value={point}
        interactive={false}
        mapFailed={mapFailed}
        onMapFailed={() => setMapFailed(true)}
        failedLabel={t("request.create.mapFailed")}
        loadingLabel={t("request.create.mapLoading")}
      />
    </Stack>
  );
}
