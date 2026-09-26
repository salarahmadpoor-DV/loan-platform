import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import {
  addMyProviderServiceArea,
  updateMyProvider,
  updateMyProviderServiceArea,
  type UpdateMyProviderBody,
} from "../api/providerProfileApi";
import { RADIUS_AREA_TYPE, type ServiceAreaSelection } from "../model/serviceArea";

export type SaveProviderServiceAreaInput = {
  selection: ServiceAreaSelection;
  existingAreaId?: number;
  profile: UpdateMyProviderBody;
};

export function useSaveProviderServiceArea() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ selection, existingAreaId, profile }: SaveProviderServiceAreaInput) => {
      const areaBody = {
        areaType: RADIUS_AREA_TYPE,
        lat: selection.lat,
        lng: selection.lng,
        radius: selection.radiusKm,
        isActive: true,
      };
      let areaId = existingAreaId;
      if (areaId) {
        await updateMyProviderServiceArea(areaId, areaBody);
      } else {
        const created = await addMyProviderServiceArea(areaBody);
        areaId = created.id;
      }
      await updateMyProvider({
        name: profile.name,
        description: profile.description,
        mobile: profile.mobile,
        lat: selection.lat,
        lng: selection.lng,
      });
      return areaId;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.myAreas() });
      await queryClient.invalidateQueries({ queryKey: queryKeys.provider.profile() });
    },
  });
}
