import { Alert, Box } from "@mui/material";
import { Component, lazy, Suspense, type ErrorInfo, type ReactNode } from "react";
import { LoadingState } from "../ui/LoadingState";
import type { MapPoint } from "./mapConstants";

const LocationLeafletMap = lazy(async () => {
  const module = await import("./LocationLeafletMap");
  return { default: module.LocationLeafletMap };
});

type MapFrameProps = {
  value: MapPoint | null;
  onSelectCenter?: (lat: number, lng: number) => void;
  circleRadiusMeters?: number;
  interactive?: boolean;
  error?: boolean;
  mapFailed: boolean;
  onMapFailed: () => void;
  failedLabel: string;
  loadingLabel: string;
};

export function MapFrame({
  value,
  onSelectCenter,
  circleRadiusMeters,
  interactive = true,
  error,
  mapFailed,
  onMapFailed,
  failedLabel,
  loadingLabel,
}: MapFrameProps) {
  if (mapFailed) {
    return <Alert severity="error">{failedLabel}</Alert>;
  }

  return (
    <Box
      dir="ltr"
      sx={{
        height: { xs: 260, sm: 320 },
        width: "100%",
        minWidth: 0,
        overflow: "hidden",
        borderRadius: 1,
        border: 1,
        borderColor: error ? "error.main" : "divider",
      }}
    >
      <Suspense fallback={<LoadingState label={loadingLabel} />}>
        <LeafletErrorBoundary onError={onMapFailed}>
          <LocationLeafletMap
            value={value}
            onSelectCenter={onSelectCenter}
            circleRadiusMeters={circleRadiusMeters}
            interactive={interactive}
          />
        </LeafletErrorBoundary>
      </Suspense>
    </Box>
  );
}

class LeafletErrorBoundary extends Component<
  { children: ReactNode; onError: () => void },
  { failed: boolean }
> {
  state = { failed: false };

  static getDerivedStateFromError(): { failed: boolean } {
    return { failed: true };
  }

  componentDidCatch(_error: Error, _info: ErrorInfo) {
    this.props.onError();
  }

  render() {
    if (this.state.failed) {
      return null;
    }
    return this.props.children;
  }
}
