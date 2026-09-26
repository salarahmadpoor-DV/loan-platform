import { Circle, CircleMarker, MapContainer, TileLayer, useMap, useMapEvents } from "react-leaflet";
import { useEffect } from "react";
import { DEFAULT_MAP_CENTER, OSM_ATTRIBUTION, OSM_TILE_URL, type MapPoint } from "./mapConstants";
import "leaflet/dist/leaflet.css";

type LocationLeafletMapProps = {
  value: MapPoint | null;
  onSelectCenter?: (lat: number, lng: number) => void;
  circleRadiusMeters?: number;
  interactive?: boolean;
};

function MapClickHandler({ onSelectCenter }: { onSelectCenter: (lat: number, lng: number) => void }) {
  useMapEvents({
    click(event) {
      onSelectCenter(event.latlng.lat, event.latlng.lng);
    },
  });
  return null;
}

function MapViewport({ lat, lng }: { lat: number; lng: number }) {
  const map = useMap();

  useEffect(() => {
    const id = window.setTimeout(() => {
      map.invalidateSize();
    }, 200);
    return () => window.clearTimeout(id);
  }, [map]);

  useEffect(() => {
    map.setView([lat, lng]);
  }, [lat, lng, map]);

  return null;
}

export function LocationLeafletMap({
  value,
  onSelectCenter,
  circleRadiusMeters,
  interactive = true,
}: LocationLeafletMapProps) {
  const lat = value?.lat ?? DEFAULT_MAP_CENTER.lat;
  const lng = value?.lng ?? DEFAULT_MAP_CENTER.lng;

  return (
    <MapContainer
      center={[lat, lng]}
      zoom={value ? 11 : 10}
      scrollWheelZoom={interactive}
      dragging={interactive}
      style={{ height: "100%", width: "100%" }}
    >
      <TileLayer attribution={OSM_ATTRIBUTION} url={OSM_TILE_URL} />
      {interactive && onSelectCenter ? <MapClickHandler onSelectCenter={onSelectCenter} /> : null}
      <MapViewport lat={lat} lng={lng} />
      {value ? (
        <>
          <CircleMarker
            center={[value.lat, value.lng]}
            radius={8}
            pathOptions={{ color: "#2563EB", fillColor: "#2563EB", fillOpacity: 1 }}
          />
          {circleRadiusMeters != null && circleRadiusMeters > 0 ? (
            <Circle
              center={[value.lat, value.lng]}
              radius={circleRadiusMeters}
              pathOptions={{ color: "#2563EB", fillColor: "#2563EB", fillOpacity: 0.15, weight: 2 }}
            />
          ) : null}
        </>
      ) : null}
    </MapContainer>
  );
}
