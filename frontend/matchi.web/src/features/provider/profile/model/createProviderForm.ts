import type { MessageKey } from "../../../../shared/i18n";
import {
  isValidServiceArea,
  type ServiceAreaSelection,
} from "./serviceArea";

export type CreateProviderFormValues = {
  name: string;
  description: string;
  serviceArea: ServiceAreaSelection | null;
};

export type CreateProviderFieldErrors = Partial<
  Record<"name" | "description" | "serviceArea", MessageKey>
>;

export const defaultCreateProviderValues: CreateProviderFormValues = {
  name: "",
  description: "",
  serviceArea: null,
};

export function validateCreateProviderForm(
  values: CreateProviderFormValues,
): CreateProviderFieldErrors {
  const errors: CreateProviderFieldErrors = {};
  const name = values.name.trim();
  if (!name) {
    errors.name = "provider.onboard.nameRequired";
  } else if (name.length > 200) {
    errors.name = "provider.onboard.nameTooLong";
  }

  if (values.description.trim().length > 2000) {
    errors.description = "provider.onboard.descriptionTooLong";
  }

  if (values.serviceArea != null && !isValidServiceArea(values.serviceArea)) {
    errors.serviceArea = "provider.serviceArea.required";
  }

  return errors;
}

export function toCreateProviderBody(values: CreateProviderFormValues): {
  name: string;
  description?: string;
  lat?: number;
  lng?: number;
} {
  const description = values.description.trim();
  const area = values.serviceArea;
  return {
    name: values.name.trim(),
    ...(description ? { description } : {}),
    ...(isValidServiceArea(area) ? { lat: area.lat, lng: area.lng } : {}),
  };
}
