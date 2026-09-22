import { addMyProviderServiceArea } from "../api/providerProfileApi";
import { RADIUS_AREA_TYPE, isValidServiceArea, type ServiceAreaSelection } from "../model/serviceArea";

export async function persistOnboardServiceArea(selection: ServiceAreaSelection | null): Promise<void> {
  if (!isValidServiceArea(selection)) {
    return;
  }
  await addMyProviderServiceArea({
    areaType: RADIUS_AREA_TYPE,
    lat: selection.lat,
    lng: selection.lng,
    radius: selection.radiusKm,
    isActive: true,
  });
}
