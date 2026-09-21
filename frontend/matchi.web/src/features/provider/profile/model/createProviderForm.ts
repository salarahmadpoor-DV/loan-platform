import type { MessageKey } from "../../../../shared/i18n";

export type CreateProviderFormValues = {
  name: string;
  description: string;
  lat: string;
  lng: string;
};

export type CreateProviderFieldErrors = Partial<Record<keyof CreateProviderFormValues, MessageKey>>;

export const defaultCreateProviderValues: CreateProviderFormValues = {
  name: "",
  description: "",
  lat: "",
  lng: "",
};

function parseOptionalNumber(raw: string): number | undefined | "invalid" {
  const trimmed = raw.trim();
  if (!trimmed) {
    return undefined;
  }
  const value = Number(trimmed);
  if (!Number.isFinite(value)) {
    return "invalid";
  }
  return value;
}

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

  const lat = parseOptionalNumber(values.lat);
  const lng = parseOptionalNumber(values.lng);
  if (lat === "invalid") {
    errors.lat = "provider.onboard.invalidCoordinate";
  }
  if (lng === "invalid") {
    errors.lng = "provider.onboard.invalidCoordinate";
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
  const lat = parseOptionalNumber(values.lat);
  const lng = parseOptionalNumber(values.lng);
  return {
    name: values.name.trim(),
    ...(description ? { description } : {}),
    ...(typeof lat === "number" ? { lat } : {}),
    ...(typeof lng === "number" ? { lng } : {}),
  };
}
